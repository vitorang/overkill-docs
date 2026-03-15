using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Core.DTOs.Users;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.States;
using System;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Cache
{
    public partial class AppCache(IMemoryCache cache) :
        IDocumentStateCache,
        IEditorStateCache,
        ISimpleUserCache,
        IUserIdentityCache
    {
        private MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
              .SetSlidingExpiration(TimeSpan.FromMinutes(10));

        private static string KeyFrom<T>(string id)
        {
            var name = typeof(T).Name;
            return $"{name}:{id}";
        }

        private static string KeyOf<T>(T value) => value switch
        {
            DocumentState v => KeyFrom<DocumentState>(v.DocumentHashId),
            EditorState v => KeyFrom<EditorState>(v.EditorId),
            SimpleUserDto v => KeyFrom<SimpleUserDto>(v.HashId),
            UserIdentity v => KeyFrom<UserIdentity>(v.Token),
            _ => throw new InvalidOperationException("Tipo não mapeado para extração de chave")
        };

        private Task<T?> GetValue<T>(string id)
        {
            var key = KeyFrom<T>(id);
            T? value = cache.Get<T>(key);

            if (value != null)
                cache.Set(key, value, options);

            return Task.FromResult(value);
        }

        private Task SetValue<T>(T value)
        {
            var key = KeyOf(value);
            cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        private Task RemoveValue<T>(T value)
        {
            var key = KeyOf(value);
            cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
