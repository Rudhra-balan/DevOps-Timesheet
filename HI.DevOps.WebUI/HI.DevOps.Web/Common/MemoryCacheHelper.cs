using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace HI.DevOps.Web.Common
{
    public static class MemoryCacheHelper
    {
        public static void SetInMemoryCache<T>(string cacheId, T viewModel, IMemoryCache memoryCache)
        {
            cacheId = cacheId.Trim();
            var cacheExpirationOption = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.UtcNow.AddHours(5),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(60)
            };
            // use a cancellation token
            var tokenSource = new CancellationTokenSource(TimeSpan.FromDays(1));
            var token = new CancellationChangeToken(tokenSource.Token);
            memoryCache.Set(cacheId, viewModel, cacheExpirationOption.AddExpirationToken(token));

        }

        public static T GetInMemoryCache<T>(string cacheId, IMemoryCache memoryCache)
        {
            cacheId = cacheId.Trim();
            memoryCache.TryGetValue(cacheId, out T model);
            return model;
        }

        public static void UpdateInMemoryCache<T>(string cacheId, T viewModel, IMemoryCache memoryCache)
        {
            cacheId = cacheId.Trim();
            memoryCache.Remove(cacheId);
            
            var cacheExpirationOption = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.UtcNow.AddHours(5),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(60)
            };
            // use a cancellation token
            var tokenSource = new CancellationTokenSource(TimeSpan.FromDays(1));
            var token = new CancellationChangeToken(tokenSource.Token);
            memoryCache.Set(cacheId, viewModel, cacheExpirationOption.AddExpirationToken(token));

        }
    }
}