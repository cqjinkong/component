using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel;

// ReSharper disable CheckNamespace

namespace Sbt.Invoice.Service
{
    /// <summary>
    /// 定时任务执行基类,继承就可以,因为这是一个单例,所以大部分的services包括db都是不能注入的.
    /// </summary>
    public class ScheduledService : IDisposable, Shashlik.Kernel.Dependency.ISingleton
    {
        private ILoggerFactory LoggerFactory { get; }
        private static ConcurrentDictionary<Type, (Timer timer, TimeSpan interval, Type timerType)> Timers { get; }

        static ScheduledService()
        {
            Timers = new ConcurrentDictionary<Type, (Timer timer, TimeSpan interval, Type timerType)>();
        }

        public ScheduledService(ILoggerFactory loggerFactory)
        {
            // 应用退出时停止定时器
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Dispose();
            LoggerFactory = loggerFactory;
        }

        public void AddTimer(ITimer timerInstance)
        {
            var timerType = timerInstance.GetType();
            var logger = LoggerFactory.CreateLogger(timerType.FullName);
            using (timerInstance)
                Timers.TryAdd(timerInstance.GetType(), (timer: new Timer(r =>
                {
                    using var scope = GlobalKernelServiceProvider.KernelServiceProvider.CreateScope();
                    using var instance = scope.ServiceProvider.GetRequiredService(timerType) as ITimer;
                    logger.LogInformation($"begin execute timer: {timerType}");
                    try
                    {
                        instance!.Execute(CancellationToken.None).Wait();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "定时任务执行失败");
                    }

                    logger.LogInformation($"execute timer done: {timerType}");
                }, null, Timeout.Infinite, 0), timerInstance.Interval, timerType));
        }

        public void Remove(Type timer)
        {
            Timers.TryRemove(timer, out _);
        }

        public void Start()
        {
            foreach (var item in Timers)
            {
                item.Value.timer.Change(TimeSpan.FromSeconds(5), item.Value.interval);
            }
        }

        public void Stop()
        {
            foreach (var item in Timers)
            {
                item.Value.timer.Change(Timeout.Infinite, 0);
            }
        }

        public void Dispose()
        {
            foreach (var item in Timers)
            {
                item.Value.timer.Dispose();
            }

            Timers.Clear();
        }
    }
}