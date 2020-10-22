using Microsoft.AspNetCore.Builder;
using Shashlik.AspNetCore;
using Shashlik.Kernel;

namespace Jinkong.Payment
{
    public class PaymentConfigure : IAutowiredConfigureAspNetCore
    {
        public void Configure(IApplicationBuilder app, IKernelConfigure kernelConfigure)
        {
            app.UsePayNotify();
        }
    }
}