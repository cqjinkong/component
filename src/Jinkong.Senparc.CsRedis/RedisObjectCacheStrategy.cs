using System;
using System.Collections.Generic;
using System.Linq;
using Senparc.CO2NET.MessageQueue;
using Senparc.CO2NET.Cache;
using System.Threading.Tasks;
using CSRedis;

namespace Jinkong.Senparc.CsRedis
{
    /// <summary>
    /// Redis的Object类型容器缓存（Key为String类型），Key-Value 类型储存
    /// </summary>
    public class RedisObjectCacheStrategy : BaseCacheStrategy, IBaseObjectCacheStrategy
    {
        public static RedisObjectCacheStrategy Instance { get; private set; }

        public CSRedisClient Client { get; }

        public static RedisObjectCacheStrategy CreateInstance(CSRedisClient csRedisClient)
        {
            return Instance ??= new RedisObjectCacheStrategy(csRedisClient);
        }

        /// <summary>
        /// Redis 缓存策略
        /// </summary>
        RedisObjectCacheStrategy(CSRedisClient csRedisClient)
        {
            Client = csRedisClient;
        }

        /// <summary>
        /// 获得过期时间（秒）
        /// </summary>
        /// <param name="expiry"></param>
        /// <returns></returns>
        private int GetExpirySeconds(TimeSpan? expiry)
        {
            var expirySeconds = expiry.HasValue ? (int) Math.Ceiling(expiry.Value.TotalSeconds) : -1;
            return expirySeconds;
        }


        #region 实现 IBaseObjectCacheStrategy 接口

        #region 同步接口

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isFullKey">是否已经是完整的Key</param>
        /// <returns></returns>
        public bool CheckExisted(string key, bool isFullKey = false)
        {
            var cacheKey = GetFinalKey(key, isFullKey);
            return Client.Exists(cacheKey);
        }

        public object Get(string key, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (!CheckExisted(key, isFullKey))
            {
                return null;
            }

            var cacheKey = GetFinalKey(key, isFullKey);

            var value = Client.Get(cacheKey);
            if (value != null)
            {
                return value.ToString().DeserializeFromCache();
            }

            return value;
        }

        public T Get<T>(string key, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key))
                return default;
            if (!CheckExisted(key, isFullKey))
                return default;
            var cacheKey = GetFinalKey(key, isFullKey);
            var value = Client.Get(cacheKey);
            if (value != null)
                return value.DeserializeFromCache<T>();

            return default;
        }

        /// <summary>
        /// 注意：此方法获取的object为直接储存在缓存中，序列化之后的Value
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetAll()
        {
            var keyPrefix = GetFinalKey(""); //获取带Senparc:DefaultCache:前缀的Key（[DefaultCache]可配置）
            var dic = new Dictionary<string, object>();

            var keys = Client.Keys( /*database: Client.GetDatabase().Database,*/
                pattern: keyPrefix + "*" /*, pageSize: 99999*/);
            foreach (var redisKey in keys)
            {
                dic[redisKey] = Get(redisKey, true);
            }

            return dic;
        }

        public long GetCount()
        {
            var keyPattern = GetFinalKey("*"); //获取带Senparc:DefaultCache:前缀的Key（[DefaultCache]         
            var count = Client.Keys( /*database: Client.GetDatabase().Database,*/
                pattern: keyPattern /*, pageSize: 99999*/).Count();
            return count;
        }

        [Obsolete("此方法已过期，请使用 Set(TKey key, TValue value) 方法")]
        public void InsertToCache(string key, object value, TimeSpan? expiry = null)
        {
            Set(key, value, expiry, false);
        }

        public void Set(string key, object value, TimeSpan? expiry = null, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                return;
            }

            var cacheKey = GetFinalKey(key, isFullKey);

            var json = value.SerializeToCache();
            Client.Set(cacheKey, json, GetExpirySeconds(expiry));
        }

        public void RemoveFromCache(string key, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var cacheKey = GetFinalKey(key, isFullKey);

            SenparcMessageQueue.OperateQueue(); //延迟缓存立即生效
            Client.Del(cacheKey); //删除键
        }

        public void Update(string key, object value, TimeSpan? expiry = null, bool isFullKey = false)
        {
            Set(key, value, expiry, isFullKey);
        }

        #endregion


        #region 异步方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isFullKey">是否已经是完整的Key</param>
        /// <returns></returns>
        public async Task<bool> CheckExistedAsync(string key, bool isFullKey = false)
        {
            var cacheKey = GetFinalKey(key, isFullKey);
            return await Client.ExistsAsync(cacheKey).ConfigureAwait(false);
        }

        public async Task<object> GetAsync(string key, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (!await CheckExistedAsync(key, isFullKey).ConfigureAwait(false))
            {
                return null;
            }

            var cacheKey = GetFinalKey(key, isFullKey);

            var value = await Client.GetAsync(cacheKey).ConfigureAwait(false);
            if (value != null)
            {
                return value.ToString().DeserializeFromCache();
            }

            return value;
        }

        public async Task<T> GetAsync<T>(string key, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            if (!await CheckExistedAsync(key, isFullKey).ConfigureAwait(false))
            {
                return default(T);
                //InsertToCache(key, new ContainerItemCollection());
            }

            var cacheKey = GetFinalKey(key, isFullKey);

            var value = await Client.GetAsync(cacheKey).ConfigureAwait(false);
            if (value != null)
            {
                return value.ToString().DeserializeFromCache<T>();
            }

            return default(T);
        }

        /// <summary>
        /// 注意：此方法获取的object为直接储存在缓存中，序列化之后的Value（最多 99999 条）
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetAllAsync()
        {
            var keyPrefix = GetFinalKey(""); //获取带Senparc:DefaultCache:前缀的Key（[DefaultCache]可配置）
            var dic = new Dictionary<string, object>();

            var keys = Client.Keys( /*database: Client.GetDatabase().Database,*/
                pattern: keyPrefix + "*" /*, pageSize: 99999*/);
            foreach (var redisKey in keys)
            {
                dic[redisKey] = await GetAsync(redisKey, true).ConfigureAwait(false);
            }

            return dic;
        }


        public Task<long> GetCountAsync()
        {
            return Task.Factory.StartNew(() => GetCount());
        }

        public async Task SetAsync(string key, object value, TimeSpan? expiry = null, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                return;
            }

            var cacheKey = GetFinalKey(key, isFullKey);

            var json = value.SerializeToCache();
            await Client.SetAsync(cacheKey, json, GetExpirySeconds(expiry)).ConfigureAwait(false);
        }

        public async Task RemoveFromCacheAsync(string key, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var cacheKey = GetFinalKey(key, isFullKey);

            SenparcMessageQueue.OperateQueue(); //延迟缓存立即生效
            await Client.DelAsync(cacheKey).ConfigureAwait(false); //删除键
        }

        public async Task UpdateAsync(string key, object value, TimeSpan? expiry = null,
            bool isFullKey = false)
        {
            await SetAsync(key, value, expiry, isFullKey).ConfigureAwait(false);
        }

        #endregion

        #endregion

        /// <summary>
        /// 根据 key 的前缀获取对象列表（最多 99999 条）
        /// </summary>
        public IList<T> GetAllByPrefix<T>(string key)
        {
            var keyPattern = GetFinalKey("*"); //获取带Senparc:DefaultCache:前缀的Key（[DefaultCache]         
            var keys = Client.Keys( /*database: Client.GetDatabase().Database,*/
                pattern: keyPattern /*, pageSize: 99999*/);
            List<T> list = new List<T>();
            foreach (var fullKey in keys)
            {
                var obj = Get<T>(fullKey, true);
                if (obj != null)
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        /// <summary>
        /// 【异步方法】根据 key 的前缀获取对象列表（最多 99999 条）
        /// </summary>
        public async Task<IList<T>> GetAllByPrefixAsync<T>(string key)
        {
            var keyPattern = GetFinalKey("*"); //获取带Senparc:DefaultCache:前缀的Key（[DefaultCache]         
            var keys = Client.Keys( /*database: Client.GetDatabase().Database,*/
                pattern: keyPattern /*, pageSize: 99999*/);
            List<T> list = new List<T>();
            foreach (var fullKey in keys)
            {
                var obj = await GetAsync<T>(fullKey, true).ConfigureAwait(false);
                if (obj != null)
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        public override ICacheLock BeginCacheLock(string resourceName, string key, int retryCount = 0,
            TimeSpan retryDelay = new TimeSpan())
        {
            return RedisCacheLock.CreateAndLock(this, resourceName, key, retryCount, retryDelay);
        }

        public override async Task<ICacheLock> BeginCacheLockAsync(string resourceName, string key, int retryCount = 0,
            TimeSpan retryDelay = new TimeSpan())
        {
            return await RedisCacheLock.CreateAndLockAsync(this, resourceName, key, retryCount, retryDelay)
                .ConfigureAwait(false);
        }
    }
}