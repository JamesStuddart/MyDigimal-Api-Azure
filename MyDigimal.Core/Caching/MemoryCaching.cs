using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace MyDigimal.Core.Caching
{
    public class MemoryCaching : ICaching
    {
        private readonly IMemoryCache _cache;

        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> Get<T>(string key, Func<Task<T>> function)
        {
            var cacheEntry = await
                _cache.GetOrCreateAsync(key, async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(3);
                    return await function();
                });

            return cacheEntry;
        }
    }

    public interface ICaching
    {
        Task<T> Get<T>(string key, Func<Task<T>> function);
    }
}