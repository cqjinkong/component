using System;
using CSRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.MP;

namespace Jinkong.Wx.AspNetCore
{
    /// <summary>
    /// 微信默认注册配置
    /// </summary>
    public class DefaultWxConfigure : IWxConfigure
    {
        public void Configure(IRegisterService registerService, IServiceProvider serviceProvider)
        {        
            // 缓存配置
            Senparc.CsRedis.Register.UseCsRedisCache(serviceProvider.GetService<CSRedisClient>());

            var appOptions = serviceProvider.GetRequiredService<IOptions<WxOptions>>();
            foreach (var item in appOptions.Value.WxAppSettings)
            {
                switch (item.Value.AppType)
                {
                    // 注册小程序和公众号数据,其他的不管
                    case WxAppType.MP:
                    case WxAppType.APPLET:
                        registerService.RegisterMpAccount(item.Value.AppId, item.Value.AppSecret, item.Key);
                        break;
                }
            }
        }
    }
}