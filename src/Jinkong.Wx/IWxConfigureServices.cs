using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Shashlik.Kernel;

namespace Jinkong.Wx
{
    /// <summary>
    /// 微信服务相关配置
    /// </summary>
    public interface IWxConfigureServices
    {
        public void ConfigureServices(IKernelServices services);
    }
}
