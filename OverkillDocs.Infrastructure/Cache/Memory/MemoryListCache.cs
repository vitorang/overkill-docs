using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Infrastructure.Interfaces;
using System.Collections.Immutable;
using System.Text.Json;

namespace OverkillDocs.Infrastructure.Cache.Memory;

internal sealed class MemoryListCache<T>(IMemoryCache cache) : ListCache<T>, IListCache<T>
{
    private readonly object sync = new();
    private static readonly MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(expirationTime);

    public Task Append(T value)
    {
        lock (sync)
        {
            var list = GetList();
            list.Add(value);
            if (list.Count > sizeLimit)
                list.RemoveRange(0, list.Count - sizeLimit);
            cache.Set(key, JsonSerializer.Serialize(list), options);
        }

        return Task.CompletedTask;
    }

    public Task<ImmutableArray<T>> Get()
    {
        lock (sync)
        {
            return Task.FromResult(GetList().ToImmutableArray());
        }
    }

    private List<T> GetList()
    {
        var json = cache.Get<string>(key) ?? "[]";
        return JsonSerializer.Deserialize<List<T>>(json)!;
    }
}
