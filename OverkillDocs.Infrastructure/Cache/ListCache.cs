using OverkillDocs.Core.Constants;
using OverkillDocs.Core.Entities.Chat;
using OverkillDocs.Core.Interfaces;
using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Cache
{
    public class ListCache<T> : IListCache<T>
    {
        private readonly List<T> cache = [];
        private DateTime lastAccessAt = DateTime.Now;
        private static readonly object sync = new();

        private static readonly TimeSpan expiration = typeof(T) switch
        {
            Type t when t == typeof(ChatMessage) => CacheConstants.ChatExpiration,
            _ => CacheConstants.DefaultObjectExpiration,
        };

        private static readonly int sizeLimit = typeof(T) switch
        {
            Type t when t == typeof(ChatMessage) => CacheConstants.ChatSize,
            _ => CacheConstants.DefaultListSize,
        };

        public Task Append(T value, CancellationToken ct)
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

        public Task<ImmutableList<T>> Get(CancellationToken ct)
        {
            lock (sync)
            {
                RefreshCache();
                return Task.FromResult(cache.ToImmutableList());
            }
        }

        private void RefreshCache()
        {
            var now = DateTime.Now;
            if (now - lastAccessAt >= expiration)
                cache.Clear();

            lastAccessAt = now;
        }
    }
}
