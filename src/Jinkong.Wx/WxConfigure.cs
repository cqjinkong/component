using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.CO2NET.HttpUtility;
using Senparc.Weixin.RegisterServices;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;

namespace Jinkong.Wx
{
    public class WxAutowire : IServiceAutowire
    {
        public WxAutowire(IOptions<WxOptions> options)
        {
            Options = options;
        }

        private IOptions<WxOptions> Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            var services = kernelService.Services;

            if (Options.Value.UseEmptyTemplateMsg)
                services.AddSingleton<IWxTemplateMsg, EmptyWxTemplateMsg>();
            else
                services.AddSingleton<IWxTemplateMsg, DefaultWxTemplateMsg>();

            services.AddSingleton<IWxAccessToken, DefaultWxAccessToken>();
            services.AddSingleton<IWxMedia, DefaultWxMedia>();
            services.AddSingleton<IWxPay, DefaultWxPay>();

            var globalOptions = Options.Value.GlobalSettings;
            if (globalOptions == null)
                throw new Exception("SdkOptions can't be null.");
            if (!globalOptions.ThisHost.IsMatch(Consts.Regexs.Url))
                throw new Exception("SdkOptions.ThisHost must be url.");

            if (!globalOptions.ProxyHost.IsNullOrWhiteSpace() && globalOptions.ProxyPort.HasValue)
            {
                if (globalOptions.ProxyUserName.IsNullOrWhiteSpace() ||
                    globalOptions.ProxyPassword.IsNullOrWhiteSpace())
                    RequestUtility.SenparcHttpClientWebProxy =
                        new WebProxy(globalOptions.ProxyHost, globalOptions.ProxyPort.Value);
                else
                {
                    NetworkCredential credential =
                        new NetworkCredential(globalOptions.ProxyUserName, globalOptions.ProxyPassword);
                    RequestUtility.SenparcHttpClientWebProxy =
                        new WebProxy($"{globalOptions.ProxyHost}:{globalOptions.ProxyPort}", true, null, credential);
                }
            }

            services
                //Senparc.CO2NET 全局注册,主要是注册下面的节点Jinkong.Wx.SenparcSetting
                .AddSenparcWeixinServices(kernelService.RootConfiguration.GetSection("Jinkong.Wx"));
        }
    }
}