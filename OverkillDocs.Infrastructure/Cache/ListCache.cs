using OverkillDocs.Core.Constants;
using OverkillDocs.Core.Entities.Chat;

namespace OverkillDocs.Infrastructure.Cache;

internal abstract class ListCache<T>
{
    protected static readonly string key = $"{typeof(T).Name}-list";

    protected static readonly TimeSpan expirationTime = typeof(T) switch
    {
        Type t when t == typeof(ChatMessage) => CacheConstants.ChatExpiration,
        _ => CacheConstants.DefaultObjectExpiration,
    };

    protected static readonly int sizeLimit = typeof(T) switch
    {
        Type t when t == typeof(ChatMessage) => CacheConstants.ChatSize,
        _ => CacheConstants.DefaultListSize,
    };
}
