using System;
using Senparc.CO2NET.RegisterServices;

namespace Jinkong.Wx.AspNetCore
{
    /// <summary>
    /// 微信配置,自动配置
    /// </summary>
    public interface IWxConfigure
    {
        void Configure(IRegisterService registerService, IServiceProvider serviceProvider);
    }
}