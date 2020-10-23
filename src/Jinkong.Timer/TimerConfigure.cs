using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

// ReSharper disable CheckNamespace

namespace Sbt.Invoice.Service
{
    public class TimerAutowire : IServiceProviderAutowire
    {
        public void Configure(IKernelServiceProvider kernelServiceProvider)
        {
            using var scope = kernelServiceProvider.CreateScope();
            var timers = scope.ServiceProvider.GetServices<ITimer>();
            var scheduledService = kernelServiceProvider.GetService<ScheduledService>();

            foreach (var item in timers)
                scheduledService.AddTimer(item);

            scheduledService.Start();
        }
    }
}