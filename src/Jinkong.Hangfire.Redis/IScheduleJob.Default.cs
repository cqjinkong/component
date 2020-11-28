using System;
using System.Linq.Expressions;
using Shashlik.Kernel.Dependency;

namespace Jinkong.Hangfire.Redis
{
    [Singleton]
    public class DefaultScheduleJob : IScheduleJob
    {
        public void Execute(Expression<Action> action, TimeSpan delay)
        {
            global::Hangfire.BackgroundJob.Schedule(action, delay);
        }

        public void Execute(Expression<Action> action, DateTimeOffset excuteAt)
        {
            global::Hangfire.BackgroundJob.Schedule(action, excuteAt);
        }
    }
}