using System;
using System.Threading.Tasks;
using CSRedis;
using Senparc.CO2NET.Cache;
using Senparc.Weixin.Annotations;
using Shashlik.Redis;

namespace Jinkong.Senparc.CsRedis
{
    public class RedisCacheLock : BaseCacheLock
    {
        // private RedLock _dlm;
        // private Lock _lockObject;
        private CSRedisClient _client;
        private IDisposable _locker;

        protected RedisCacheLock(IBaseCacheStrategy strategy, string resourceName, string key, int? retryCount,
            TimeSpan? retryDelay)
            : base(strategy, resourceName, key, retryCount, retryDelay)
        {
            _client = (strategy as RedisObjectCacheStrategy)!.Client;
            //LockNow();//立即等待并抢夺锁
        }


        /// <summary>
        /// 创建 RedisCacheLock 实例，并立即尝试获得锁
        /// </summary>
        /// <param name="strategy">BaseRedisObjectCacheStrategy</param>
        /// <param name="resourceName"></param>
        /// <param name="key"></param>
        /// <param name="retryCount"></param>
        /// <param name="retryDelay"></param>
        /// <returns></returns>
        public static ICacheLock CreateAndLock(IBaseCacheStrategy strategy, string resourceName, string key,
            int? retryCount = null, TimeSpan? retryDelay = null)
        {
            return new RedisCacheLock(strategy, resourceName, key, retryCount, retryDelay)
                .Lock();
        }

        public override ICacheLock Lock()
        {
            try
            {
                var ttl = GetTotalTtl(_retryCount, _retryDelay);
                _locker = _client.Lock(_resourceName, (int) TimeSpan.FromMilliseconds(ttl).TotalSeconds);
            }
            catch
            {
                LockSuccessful = false;
            }

            return this;
        }

        public override void UnLock()
        {
            _locker?.Dispose();
        }


        /// <summary>
        /// 【异步方法】创建 RedisCacheLock 实例，并立即尝试获得锁
        /// </summary>
        /// <param name="strategy">BaseRedisObjectCacheStrategy</param>
        /// <param name="resourceName"></param>
        /// <param name="key"></param>
        /// <param name="retryCount"></param>
        /// <param name="retryDelay"></param>
        /// <returns></returns>
        public static async Task<ICacheLock> CreateAndLockAsync(IBaseCacheStrategy strategy, string resourceName,
            string key, int? retryCount = null, TimeSpan? retryDelay = null)
        {
            return await new RedisCacheLock(strategy, resourceName, key, retryCount,
                retryDelay).LockAsync().ConfigureAwait(false);
        }


        public override async Task<ICacheLock> LockAsync()
        {
            try
            {
                var ttl = GetTotalTtl(_retryCount, _retryDelay);
                _locker = _client.Lock(_resourceName, (int) TimeSpan.FromMilliseconds(ttl).TotalSeconds);
            }
            catch
            {
                LockSuccessful = false;
            }

            return await Task.FromResult(this);
        }

        public override async Task UnLockAsync()
        {
            await Task.CompletedTask;
            _locker?.Dispose();
        }
    }
}