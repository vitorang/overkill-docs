using OverkillDocs.Infrastructure.Interfaces;
using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Cache.Memory
{
    public class MemoryListCache<T> : ListCache<T>, IListCache<T>
    {
        private static readonly List<T> cache = [];
        private static DateTime lastAccessAt = DateTime.Now;
        private static readonly object sync = new();

        public Task Append(T value)
        {
            lock (sync)
            {
                RefreshCache();

                cache.Add(value);
                if (cache.Count > sizeLimit)
                    cache.RemoveRange(0, cache.Count - sizeLimit);
            }

            return Task.CompletedTask;
        }

        public Task<ImmutableArray<T>> Get()
        {
            lock (sync)
            {
                RefreshCache();
                return Task.FromResult(cache.ToImmutableArray());
            }
        }

        private static void RefreshCache()
        {
            var now = DateTime.Now;
            if (now - lastAccessAt >= expirationTime)
                cache.Clear();

            lastAccessAt = now;
        }
    }
}
