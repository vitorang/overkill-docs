using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Cache.Memory;

internal sealed class MemoryObjectCache<T>(IMemoryCache cache) : ObjectCache<T>, IObjectCache<T>
{
    private readonly MemoryCacheEntryOptions options = new() { SlidingExpiration = expirationTime, };

    public Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss) => Get(id.ToString(), onCacheMiss);
    public async Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss)
    {
        var key = KeyFrom(id);
        T? value = default;

        var strValue = cache.Get<string?>(key);
        if (!string.IsNullOrEmpty(strValue))
            value = JsonToEntity(strValue);

        if (value == null && onCacheMiss != null)
        {
            value = await onCacheMiss();
            if (value != null)
                await Set(value);
        }

        return value;
    }

    public Task<T[]> GetAll(int[] ids) => GetAll([.. ids.Select(e => e.ToString())]);

    public async Task<T[]> GetAll(string[] ids)
    {
        var tasks = ids.Select(id => Get(id, onCacheMiss: null));
        var results = await Task.WhenAll(tasks);

        return [.. results.Where(e => e != null)!];
    }

    public Task Set(T value)
    {
        var key = KeyOf(value);
        var json = EntityToJson(value);

        cache.Set(key, json, options);
        return Task.CompletedTask;
    }

    public Task Remove(T value, bool ifEquals)
    {
        var key = KeyOf(value);

        if (!ifEquals || EntityToJson(value) == cache.Get<string?>(key))
            cache.Remove(key);

        return Task.CompletedTask;
    }

    public Task RemoveById(int id) => RemoveById(id.ToString());

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

    public async Task<bool> CreateOrRenew(T value)
    {
        var key = KeyOf(value);
        var oldValue = cache.Get<string?>(key);
        var newValue = EntityToJson(value);

        if (oldValue == null || oldValue == newValue)
        {
            await Set(value);
            return true;
        }

        return false;
    }
}
