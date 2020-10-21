using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

// ReSharper disable CheckNamespace

namespace Sbt.Invoice.Service
{
    public class TimerConfigure : IAutowiredConfigure
    {
        public void Configure(IKernelConfigure kernelConfigure)
        {
            using var scope = kernelConfigure.ServiceProvider.CreateScope();
            var timers = scope.ServiceProvider.GetServices<ITimer>();
            var scheduledService = kernelConfigure.ServiceProvider.GetService<ScheduledService>();

            foreach (var item in timers)
                scheduledService.AddTimer(item);

            scheduledService.Start();
        }
    }
}