using CSRedis;
using Senparc.CO2NET.Cache;

namespace Jinkong.Senparc.CsRedis
{
    /// <summary>
    /// Redis 注册
    /// </summary>
    public static class Register
    {
        /// <summary>
        /// 注册键值对 redis缓存策略
        /// </summary>
        /// <param name="csRedisClient"></param>
        public static void UseCsRedisCache(CSRedisClient csRedisClient)
        {
            CacheStrategyFactory.RegisterObjectCacheStrategy(() =>
                RedisObjectCacheStrategy.CreateInstance(csRedisClient)); //键值Redis

            RedisContainerCacheStrategy.InitInstance(csRedisClient);
        }
    }
}