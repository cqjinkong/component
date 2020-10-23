using System;
using Senparc.CO2NET.RegisterServices;
using Shashlik.Kernel;

namespace Jinkong.Wx.AspNetCore
{
    /// <summary>
    /// 微信配置,自动配置
    /// </summary>
    public interface IWxConfigureExtensionAutowire : IAutowire
    {
        void Configure(IRegisterService registerService, IServiceProvider serviceProvider);
    }
}