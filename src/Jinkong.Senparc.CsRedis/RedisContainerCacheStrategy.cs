using System.Collections.Generic;
using System.Threading.Tasks;
using CSRedis;
using Senparc.CO2NET.Cache;
using Senparc.Weixin.Cache;
using Senparc.Weixin.Helpers;

// ReSharper disable CheckNamespace

namespace Jinkong.Senparc.CsRedis
{
    /// <summary>
    /// Redis容器缓存策略
    /// </summary>
    public sealed class RedisContainerCacheStrategy : BaseContainerCacheStrategy
    {
        public override ICacheStrategyDomain CacheStrategyDomain => ContainerCacheStrategyDomain.Instance;

        public CSRedisClient Client { get; }

        /// <summary>
        /// Redis 缓存策略
        /// </summary>
        private RedisContainerCacheStrategy(CSRedisClient csRedisClient)
        {
            Client = csRedisClient;
            BaseCacheStrategy = () => RedisObjectCacheStrategy.Instance;
            RegisterCacheStrategyDomain(this);
        }

        //静态SearchCache
        public static RedisContainerCacheStrategy Instance { get; private set; }

        public static void InitInstance(CSRedisClient client)
        {
            Instance = new RedisContainerCacheStrategy(client);
        }

        /// <summary>
        ///  获取所有 Bag 对象
        /// </summary>
        /// <typeparam name="TBag"></typeparam>
        /// <returns></returns>
        public override IDictionary<string, TBag> GetAll<TBag>()
        {
            var baseCacheStrategy = BaseCacheStrategy();
            var key = ContainerHelper.GetItemCacheKey(typeof(TBag), "");
            key = key.Substring(0, key.Length - 1); //去掉:号
            key = baseCacheStrategy.GetFinalKey(key); //获取带SenparcWeixin:DefaultCache:前缀的Key（[DefaultCache]可配置）
            var list = (baseCacheStrategy as RedisObjectCacheStrategy)!.GetAllByPrefix<TBag>(key);

            //var list = (baseCacheStrategy as RedisObjectCacheStrategy).GetAll(key);
            var dic = new Dictionary<string, TBag>();

            foreach (var item in list)
            {
                var fullKey = key + ":" + item.Key; //最完整的finalKey（可用于LocalCache），还原完整Key，格式：[命名空间]:[Key]
                //dic[fullKey] = StackExchangeRedisExtensions.Deserialize<TBag>(hashEntry.Value);
                dic[fullKey] = item;
            }

            return dic;
        }

        /// <summary>
        ///  【异步方法】获取所有 Bag 对象
        /// </summary>
        /// <typeparam name="TBag"></typeparam>
        /// <returns></returns>
        public override async Task<IDictionary<string, TBag>> GetAllAsync<TBag>()
        {
            var baseCacheStrategy = BaseCacheStrategy();
            var key = ContainerHelper.GetItemCacheKey(typeof(TBag), "");
            key = key.Substring(0, key.Length - 1); //去掉:号
            key = baseCacheStrategy.GetFinalKey(key); //获取带SenparcWeixin:DefaultCache:前缀的Key（[DefaultCache]可配置）
            var list = await (baseCacheStrategy as RedisObjectCacheStrategy)!.GetAllByPrefixAsync<TBag>(key)
                .ConfigureAwait(false);

            //var list = (baseCacheStrategy as RedisObjectCacheStrategy).GetAll(key);
            var dic = new Dictionary<string, TBag>();

            foreach (var item in list)
            {
                var fullKey = key + ":" + item.Key; //最完整的finalKey（可用于LocalCache），还原完整Key，格式：[命名空间]:[Key]
                //dic[fullKey] = StackExchangeRedisExtensions.Deserialize<TBag>(hashEntry.Value);
                dic[fullKey] = item;
            }

            return dic;
        }
    }
}