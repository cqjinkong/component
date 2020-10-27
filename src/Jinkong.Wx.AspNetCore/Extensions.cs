using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Jinkong.Wx.AspNetCore
{
    public static class Extensions
    {
        /// <summary>
        /// 使用微信api
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static void UseWxApi(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<WxAspNetCoreOptions>>().Value;

            if (!options.WxJsSdk.IsNullOrWhiteSpace()) app.Map(options.WxJsSdk, r => r.Run(WxJsSdk));
            if (!options.OAuthUrl.IsNullOrWhiteSpace()) app.Map(options.OAuthUrl, r => r.Run(OAuthUrl));
            if (!options.WxServerResponse.IsNullOrWhiteSpace())
                app.Map(options.WxServerResponse, r => r.Run(WxServerResponse));
        }

        static async Task OAuthUrl(HttpContext httpContext)
        {
            string appName = null, returnUrl, state = "jinkong-wxoautu-state";
            if (httpContext.Request.Query.TryGetValue("app", out var app))
                appName = app.ToString();
            if (httpContext.Request.Query.TryGetValue("returnUrl", out var returnUrlv))
                returnUrl = returnUrlv.ToString();
            else
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            if (httpContext.Request.Query.TryGetValue("state", out var stateV))
                state = stateV.ToString();

            var options = httpContext.RequestServices.GetRequiredService<IOptions<WxOptions>>().Value;
            var mpOptions = appName.IsNullOrWhiteSpace() ? options.GetDefaultMp() : options.Get(appName);
            if (mpOptions == null || mpOptions.AppType != WxAppType.MP)
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            var url = OAuthApi.GetAuthorizeUrl(mpOptions.AppId, returnUrl.UrlEncode(), state,
                OAuthScope.snsapi_userinfo);

            await httpContext.Response.WriteAsync(url);
        }

        /// <summary>
        /// 获取jssdk配置对象
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        static async Task WxJsSdk(HttpContext httpContext)
        {
            string appName = null, url;
            if (httpContext.Request.Query.TryGetValue("app", out var app))
                appName = app.ToString();
            if (httpContext.Request.Query.TryGetValue("url", out var urlV))
                url = urlV.ToString();
            else
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            var options = httpContext.RequestServices.GetRequiredService<IOptions<WxOptions>>().Value;
            var mpOptions = appName.IsNullOrWhiteSpace() ? options.GetDefaultMp() : options.Get(appName);
            if (mpOptions == null || mpOptions.AppType != WxAppType.MP)
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            var jsSdkUiPackage = await JSSDKHelper.GetJsSdkUiPackageAsync(mpOptions.AppId, mpOptions.AppSecret, url);
            await httpContext.WriteJson(jsSdkUiPackage);
        }

        static async Task WriteText(this HttpContext httpContext, string content,
            string contentType = "text/plain; charset=utf-8")
        {
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(content);
        }

        static async Task WriteXml(this HttpContext httpContext, string content,
            string contentType = "text/xml; charset=utf-8")
        {
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(content);
        }

        static async Task WriteJson<T>(this HttpContext httpContext, T result,
            string contentType = "application/json; charset=utf-8")
            where T : class
        {
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(result.ToJsonWithCamelCase());
        }

        /// <summary>
        /// 使用默认公众号
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        static async Task WxServerResponse(HttpContext httpContext)
        {
            if (httpContext.Request.Method.EqualsIgnoreCase("get"))
            {
                // get请求用于验证
                httpContext.Request.Query.TryGetValue("echostr", out var echoStr);
                await WriteText(httpContext, echoStr);
                return;
            }
            else
            {
                try
                {
                    httpContext.Request.Query.TryGetValue("signature", out var signature);
                    httpContext.Request.Query.TryGetValue("timestamp", out var timestamp);
                    httpContext.Request.Query.TryGetValue("nonce", out var nonce);
                    httpContext.Request.Query.TryGetValue("msg_signature", out var msgSignature);
                    var wxSettings = httpContext.RequestServices.GetRequiredService<IOptions<WxOptions>>().Value;
                    var wxApiOptions = httpContext.RequestServices.GetRequiredService<IOptions<WxAspNetCoreOptions>>().Value;

                    var defaultMp = wxSettings.GetDefaultMp();
                    PostModel postModel = new PostModel
                    {
                        Signature = signature,
                        Timestamp = timestamp,
                        Nonce = nonce,
                        Token = defaultMp.Token,
                        EncodingAESKey = defaultMp.EncodingAESKey,
                        AppId = defaultMp.AppId,
                        Msg_Signature = msgSignature
                    };

                    if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce,
                        postModel.Token))
                    {
                        await WriteText(httpContext, "success");
                        return;
                    }

                    var messageHandler = new WxMessageHandler(httpContext, httpContext.Request.GetRequestMemoryStream(),
                        postModel, 10);
                    await messageHandler.ExecuteAsync(CancellationToken.None);

                    if (messageHandler.ResponseMessage == null
                        || messageHandler.FinalResponseDocument == null
                        || messageHandler.ResponseMessage is ResponseMessageNoResponse)
                    {
                        await WriteText(httpContext, "success");
                        return;
                    }

                    // 如果请求需要转发
                    if (messageHandler.ResponseMessage is ResponseRedirect responseRedirect)
                    {
                        var url =
                            $"{(responseRedirect).Host.TrimEnd('/')}{wxApiOptions.WxServerResponse}" +
                            httpContext.Request.QueryString.Value;
                        // 这里没有form参数
                        var body = httpContext.Request.Body.ReadToString();
                        var res = await HttpHelper.Post(url, body, httpContext.Request.ContentType);
                        await WriteText(httpContext, res.Content);
                        return;
                    }

                    // 输出xml响应
                    await WriteText(httpContext, messageHandler.FinalResponseDocument.ToString());
                }
                catch (Exception ex)
                {
                    var logger = httpContext.RequestServices.GetService<ILoggerFactory>()
                        .CreateLogger(nameof(WxServerResponse));
                    logger.LogError(ex, "微信服务器响应错误");
                    await WriteText(httpContext, "success");
                }
            }
        }
    }
}