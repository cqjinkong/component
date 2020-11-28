using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jinkong.SecurityProxyClient;
using Jinkong.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;

namespace Jinkong.SecurityProxyApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class RootController : ControllerBase
    {
        public RootController(IOptions<Settings> options)
        {
            Settings = options.Value;
        }

        private Settings Settings { get; }

        [HttpPost("/proxy")]
        [AllowAnonymous]
        public async Task<IActionResult> Proxy(RequestObj requestObj, [FromServices] ILogger<RootController> logger)
        {
            if (!Settings.IpWhite.IsNullOrWhiteSpace() && Settings.IpWhite != "*"
                                                       && !Settings.IpWhiteList.Value.Contains(HttpContext.Connection
                                                           .RemoteIpAddress.ToString()))
                return BadRequest("invalid ip address.");

            if (!Settings.AllowTargets.IsNullOrWhiteSpace() && Settings.AllowTargets != "*")
            {
                var uri = new Uri(requestObj.TargetUrl.Trim());

                if (!Settings.AllowTargetList.Value.Contains($"{uri.Scheme}://{uri.Authority}",
                    StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("invalid target.");
                }
            }

            try
            {
                var decodedStr = SecretHelper.Decrypt(requestObj.Body, Settings.LocalPrivateKey, Encoding.GetEncoding(requestObj.Encode));
                if (!SecretHelper.Verify(decodedStr, requestObj.Signature, Settings.RemotePublicKey, Encoding.GetEncoding(requestObj.Encode)))
                    return BadRequest("signature error.");
                var method = requestObj.Method ?? RestSharp.Method.POST;
                if (decodedStr == "empty")
                    decodedStr = "";
                var res = await HttpHelper.DoRequest(method, requestObj.TargetUrl, decodedStr, requestObj.ContentType,
                    requestObj.QueryStrings, requestObj.Headers, requestObj.Cookies,
                    requestObj.Timeout ?? 60, proxy: null, encoding: Encoding.GetEncoding(requestObj.Encode));

                if (!res.IsSuccessful)
                {
                    logger.LogError(
                        $"request {requestObj.TargetUrl} response error, method: ${method}, http code:{res.StatusCode}, detail: {res.Content}");
                    return BadRequest(res.Content);
                }

                await using var ms = new MemoryStream(res.RawBytes);
                string response;
                if (res.RawBytes.Length == 0)
                    response = "empty";
                else
                    response = await ms.ReadToStringAsync(Encoding.GetEncoding(requestObj.Encode));
                var resBody = SecretHelper.Encrypt(response, Settings.RemotePublicKey, Encoding.GetEncoding(requestObj.Encode));
                var resSignature = SecretHelper.Sign(response, Settings.LocalPrivateKey, Encoding.GetEncoding(requestObj.Encode));
                return new JsonResult(new ResponseObj
                {
                    Body = resBody,
                    Signature = resSignature
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/ip")]
        [AllowAnonymous]
        public string Ip()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}