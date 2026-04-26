using OverkillDocs.Infrastructure.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace OverkillDocs.Infrastructure.Cache.Redis;

internal sealed class RedisObjectCache<T>(IConnectionMultiplexer redis) : ObjectCache<T>, IObjectCache<T>
{
    private readonly IDatabase database = redis.GetDatabase();
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss = null) => Get(id.ToString(), onCacheMiss);

    public async Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss = null)
    {
        var key = KeyFrom(id);
        var stringGet = database.StringGetAsync(key);
        var keyExpire = database.KeyExpireAsync(key, expirationTime);
        T? value = default;

        await Task.WhenAll(stringGet, keyExpire);
        var result = await stringGet;

        if (result.HasValue)
            value = ToEntity(result);
        else if (onCacheMiss != null)
        {
            await semaphore.WaitAsync();
            try
            {
                result = await database.StringGetAsync(key);
                if (result.HasValue)
                    value = ToEntity(result);
                else
                {
                    value = await onCacheMiss();
                    if (value != null)
                        await database.StringSetAsync(key, ToJsonString(value!), expirationTime);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        return value;
    }

    public async Task Remove(T value)
    {
        var key = KeyOf(value);
        await database.KeyDeleteAsync(key);
    }

    public async Task RemoveById(string id)
    {
        var key = KeyFrom(id);
        await database.KeyDeleteAsync(key);
    }

    public async Task Set(T value)
    {
        var key = KeyOf(value);
        await database.StringSetAsync(key, ToJsonString(value), expirationTime);
    }

    private static string ToJsonString(T value)
    {
        return JsonSerializer.Serialize(value, jsonOptions);
    }

    private static T? ToEntity(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return default;
        return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }

    public async Task RemoveAll(IEnumerable<T> values)
    {
        var batch = database.CreateBatch();
        var tasks = values
            .Select(e => batch.KeyDeleteAsync(KeyOf(e)))
            .ToArray();

        batch.Execute();
        await Task.WhenAll(tasks);
    }
}
