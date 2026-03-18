using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Core.Constants;
using OverkillDocs.Core.Entities.Chat;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Interfaces;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Cache
{
    public partial class AppCache<T>(IMemoryCache cache) : IAppCache<T>
    {
        private static readonly MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(typeof(T) switch
            {
                Type t when t == typeof(ChatHistory) => CacheConstants.ChatExpiration,
                _ => CacheConstants.DefaultExpiration,
            });

        private static string KeyFrom(string id)
        {
            var name = typeof(T).Name;
            return $"{name}:{id}";
        }

        private static string KeyFrom(int id) => KeyFrom(id.ToString());

        private static string KeyOf(T value) => value switch
        {
            ChatHistory v => KeyFrom(v.Id),
            UserIdentity v => KeyFrom(v.Token),
            User v => KeyFrom(v.Id),
            _ => throw new InvalidOperationException("Tipo não mapeado para criação de chave")
        };

        public async Task<T?> Get(string id, CancellationToken ct, Func<Task<T?>>? onCacheMiss = null)
        {
            var key = KeyFrom(id);
            var value = cache.Get<T>(key);

            if (value == null && onCacheMiss != null)
                value = await onCacheMiss();

            if (value != null)
                cache.Set(key, value, options);

            return value;
        }

        public Task<T?> Get(int id, CancellationToken ct, Func<Task<T?>>? onCacheMiss = null) => Get(id.ToString(), ct, onCacheMiss);

        public Task Set(T value, CancellationToken ct)
        {
            var key = KeyOf(value);
            cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task Remove(T value, CancellationToken ct)
        {
            var key = KeyOf(value);
            cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveById(string id, CancellationToken ct)
        {
            var key = KeyFrom(id);
            cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
