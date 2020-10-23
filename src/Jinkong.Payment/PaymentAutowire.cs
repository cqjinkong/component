using Microsoft.AspNetCore.Builder;
using Shashlik.AspNetCore;
using Shashlik.Kernel;

namespace Jinkong.Payment
{
    public class PaymentAutowire : IAspNetCoreAutowire
    {
        public void Configure(IApplicationBuilder app, IKernelServiceProvider kernelServiceProvider)
        {
            app.UsePayNotify();
        }
    }
}