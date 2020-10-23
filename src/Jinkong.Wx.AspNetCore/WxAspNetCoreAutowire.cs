using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.CO2NET.AspNet;
using Shashlik.AspNetCore;
using Shashlik.Kernel;

namespace Jinkong.Wx.AspNetCore
{
    public class WxAspNetCoreAutowire : IAspNetCoreAutowire
    {
        public WxAspNetCoreAutowire(IOptions<WxOptions> wxOptions, IOptions<WxAspNetCoreOptions> wxApiOptions)
        {
            WxOptions = wxOptions;
            WxApiOptions = wxApiOptions;
        }

        private IOptions<WxAspNetCoreOptions> WxApiOptions { get; set; }
        private IOptions<WxOptions> WxOptions { get; set; }

        public void Configure(IApplicationBuilder app, IKernelServiceProvider kernelServiceProvider)
        {
            if (!WxOptions.Value.Enable)
                return;

            {
                var senparcSetting = app.ApplicationServices.GetRequiredService<IOptions<SenparcSetting>>();
                var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

                app.UseSenparcGlobal(env, senparcSetting.Value,
                    globalRegister =>
                    {
                        kernelServiceProvider.Autowire<IWxConfigureExtensionAutowire>(
                            r => r.Configure(globalRegister, app.ApplicationServices));
                    });
            }

            app.UseWxApi();
        }
    }
}