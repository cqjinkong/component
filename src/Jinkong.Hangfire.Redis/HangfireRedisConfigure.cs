using CSRedis;
using Hangfire;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Redis;

namespace Jinkong.Hangfire.Redis
{
    [AfterAt(typeof(RedisAutowire))]
    public class HangfireRedisAutowire : IServiceAutowire
    {
        public HangfireRedisAutowire(IOptions<HangfireOptions> options, CSRedisClient redisClient)
        {
            Options = options;
            RedisClient = redisClient;
        }

        private IOptions<HangfireOptions> Options { get; }
        private CSRedisClient RedisClient { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            kernelService.Services.AddHangfire(r =>
                {
                    r.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseRedisStorage(RedisClient);
                })
                .AddHangfireServer();
        }
    }
}