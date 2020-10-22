using System;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.AspNetCore;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Jinkong.Hangfire.AspNetCore
{
    public class HangfireAspNetCoreConfire : IAutowiredConfigureAspNetCore
    {
        public HangfireAspNetCoreConfire(IOptions<HangfireOptions> options)
        {
            Options = options;
        }

        private IOptions<HangfireOptions> Options { get; }

        public void Configure(IApplicationBuilder app, IKernelConfigure kernelConfigure)
        {
            app.UseHangfireServer();
            if (Options.Value.Enable)
                app.UseHangfireDashboard();

            // 得到所有的循环任务
            var allRecurringJob = app.ApplicationServices.GetServices<IRecurringJob>().ToList();
            if (!allRecurringJob.IsNullOrEmpty())
                // 统一添加到循环任务执行机
                foreach (var item in allRecurringJob)
                    RecurringJob.AddOrUpdate(item.JobId, () => item.Execute(), item.CronExpression, TimeZoneInfo.Local);
        }
    }
}