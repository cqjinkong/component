using System;
using System.Linq;
using Jinkong.Wx.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.RegisterServices;
using Senparc.Weixin.TenPay;
using Shashlik.Kernel;

namespace Jinkong.Payment
{
    /// <summary>
    /// 微信支付配置注册
    /// </summary>
    public class WxPayConfigure : IWxConfigureExtensionAutowire, IServiceAutowire
    {
        public void Configure(IRegisterService registerService, IServiceProvider serviceProvider)
        {
            var mchs = serviceProvider.GetService<IOptions<PayMchOptions>>();

            foreach (var item in mchs.Value.Mchs)
            {
                var wxMch = item.Value.FirstOrDefault(r => r.PayType == PayType.WxPay);
                if (wxMch == null)
                    continue;

                registerService.RegisterTenpayV3(() => new global::Senparc.Weixin.TenPay.V3.TenPayV3Info(
                        null, null, wxMch.MchId, wxMch.MchKey, wxMch.WxCertPath, wxMch.WxCertPwd, null, null
                    )
                    , item.Key);
            }
        }

        public void Configure(IKernelServices kernelServices)
        {
            var services = kernelServices.Services;
            using var serviceProvider = services.BuildServiceProvider();
            var mchs = serviceProvider.GetService<IOptions<PayMchOptions>>();

            foreach (var item in mchs.Value.Mchs)
            {
                var wxMch = item.Value.FirstOrDefault(r => r.PayType == PayType.WxPay);
                if (wxMch == null)
                    continue;

                //  需要先注册带证书的httpclient
                var key = TenPayHelper.GetRegisterKey(wxMch.MchId, null);
                services.AddCertHttpClient(key, wxMch.WxCertPwd, wxMch.WxCertPath);
            }
        }
    }
}