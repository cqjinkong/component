using System;
using System.Linq.Expressions;

namespace Jinkong.Hangfire.Redis
{
    public class DefaultScheduleJob : IScheduleJob, Shashlik.Kernel.Dependency.ISingleton
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