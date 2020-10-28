using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Shashlik.Utils.Extensions;

namespace Jinkong.Mail
{
    public static class Extensions
    {
        internal static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key) where T : class
        {
            var content = await cache.GetStringAsync(key);
            if (content.IsNullOrWhiteSpace())
                return null;
            return JsonConvert.DeserializeObject<T>(content);
        }

        internal static async Task SetObjectAsync(this IDistributedCache cache, string key, object obj, int expireSeconds)
        {
            await cache.SetStringAsync(key, obj.ToJson(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            });
        }
    }
}
