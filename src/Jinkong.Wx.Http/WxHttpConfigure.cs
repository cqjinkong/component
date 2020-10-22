using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.CO2NET.AspNet;
using Shashlik.AspNetCore;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Jinkong.Wx.Http
{
    public class WxHttpConfigure : IAutowiredConfigureAspNetCore
    {
        public WxHttpConfigure(IOptions<WxOptions> wxOptions, IOptions<WxApiOptions> wxApiOptions)
        {
            WxOptions = wxOptions;
            WxApiOptions = wxApiOptions;
        }

        private IOptions<WxApiOptions> WxApiOptions { get; set; }
        private IOptions<WxOptions> WxOptions { get; set; }

        public void Configure(IApplicationBuilder app, IKernelConfigure kernelConfigure)
        {
            if (!WxOptions.Value.Enable)
                return;

            {
                var senparcSetting = app.ApplicationServices.GetRequiredService<IOptions<SenparcSetting>>();
                var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

                app.UseSenparcGlobal(env, senparcSetting.Value,
                        globalRegister =>
                        {
                            kernelConfigure.BeginAutowired<IWxConfigure>()
                                .Build(r =>
                                {
                                    (r.ServiceInstance as IWxConfigure)!.Configure(globalRegister,
                                        app.ApplicationServices);
                                });
                        })
                    ;
            }

            app.UseWxApi();
        }
    }
}