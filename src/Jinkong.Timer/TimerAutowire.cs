using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;

// ReSharper disable CheckNamespace

namespace Sbt.Invoice.Service
{
    [ConditionOnProperty(typeof(bool), "Jinkong.Timer.Enable", true, DefaultValue = true)]
    [Order(120)]
    public class TimerAutowire : IApplicationStartAutowire
    {
        public TimerAutowire(ScheduledService scheduledService, IEnumerable<ITimer> timers)
        {
            ScheduledService = scheduledService;
            Timers = timers;
        }

        private ScheduledService ScheduledService { get; }
        private IEnumerable<ITimer> Timers { get; }

        public async Task OnStart(CancellationToken cancellationToken)
        {
            foreach (var item in Timers)
                ScheduledService.AddTimer(item);

            ScheduledService.Start();

            await Task.CompletedTask;
        }
    }
}