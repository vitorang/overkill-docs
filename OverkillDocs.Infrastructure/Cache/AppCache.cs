using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.States;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Cache
{
    public partial class AppCache<T>(IMemoryCache cache) : IAppCache<T>
    {
        private static readonly MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                      .SetSlidingExpiration(TimeSpan.FromMinutes(10));

        private static string KeyFrom(string id)
        {
            var name = typeof(T).Name;
            return $"{name}:{id}";
        }

        private static string KeyFrom(int id) => KeyFrom(id.ToString());

        private static string KeyOf(T value) => value switch
        {
            DocumentState v => KeyFrom(v.DocumentHashId),
            EditorState v => KeyFrom(v.EditorId),
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
