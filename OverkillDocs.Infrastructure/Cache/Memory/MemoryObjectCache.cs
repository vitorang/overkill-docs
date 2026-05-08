using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Infrastructure.Interfaces;
using System.Text.Json;

namespace OverkillDocs.Infrastructure.Cache.Memory;

internal sealed class MemoryObjectCache<T>(IMemoryCache cache) : ObjectCache<T>, IObjectCache<T>
{
    private readonly object _lock = new();
    private CancellationTokenSource cts = new();
    private MemoryCacheEntryOptions GetOptions()
    {
        var options = new MemoryCacheEntryOptions()
        {
            SlidingExpiration = expirationTime,
        };

        options.ExpirationTokens.Add(new Microsoft.Extensions.Primitives.CancellationChangeToken(cts.Token));
        return options;
    }

    public async Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss = null)
    {
        var key = KeyFrom(id);
        T? value = default;

        var strValue = cache.Get<string?>(key);
        if (!string.IsNullOrEmpty(strValue))
            value = JsonSerializer.Deserialize<T?>(strValue);

        if (value == null && onCacheMiss != null)
        {
            value = await onCacheMiss();
            if (value != null)
                await Set(value);
        }

        return value;
    }

    public Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss = null) => Get(id.ToString(), onCacheMiss);

    public Task Set(T value)
    {
        var key = KeyOf(value);
        var json = JsonSerializer.Serialize(value);

        cache.Set(key, json, GetOptions());
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

    public Task Clear()
    {
        lock (_lock)
        {
            cts.Cancel();
            cts.Dispose();
            cts = new();
        }
        return Task.CompletedTask;
    }
}
