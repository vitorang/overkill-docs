using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Cache.Memory
{
    public partial class MemoryObjectCache<T>(IMemoryCache cache) : ObjectCache<T>, IObjectCache<T>
    {
        private static readonly MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(expirationTime);

        public async Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss = null)
        {
            var key = KeyFrom(id);
            var value = cache.Get<T>(key);

            if (value == null && onCacheMiss != null)
                value = await onCacheMiss();

            if (value != null)
                cache.Set(key, value, options);

            return value;
        }

        public Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss = null) => Get(id.ToString(), onCacheMiss);

        public Task Set(T value)
        {
            var key = KeyOf(value);
            cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task Remove(T value)
        {
            var key = KeyOf(value);
            cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveById(string id)
        {
            var key = KeyFrom(id);
            cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveAll(IEnumerable<T> values)
        {
            foreach (var value in values)
                cache.Remove(KeyOf(value));
            return Task.CompletedTask;
        }
    }
}
