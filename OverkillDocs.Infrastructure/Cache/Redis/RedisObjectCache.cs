using OverkillDocs.Infrastructure.Interfaces;
using StackExchange.Redis;

namespace OverkillDocs.Infrastructure.Cache.Redis;

internal sealed class RedisObjectCache<T>(IConnectionMultiplexer redis) : ObjectCache<T>, IObjectCache<T>
{
    private readonly IDatabase database = redis.GetDatabase();
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss) => Get(id.ToString(), onCacheMiss);

    public async Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss)
    {
        var key = KeyFrom(id);
        var stringGet = database.StringGetAsync(key);
        var keyExpire = database.KeyExpireAsync(key, expirationTime);
        T? value = default;

        await Task.WhenAll(stringGet, keyExpire);
        var result = await stringGet;

        if (result.HasValue)
            value = JsonToEntity(result);
        else if (onCacheMiss != null)
        {
            await semaphore.WaitAsync();
            try
            {
                result = await database.StringGetAsync(key);
                if (result.HasValue)
                    value = JsonToEntity(result);
                else
                {
                    value = await onCacheMiss();
                    if (value != null)
                        await database.StringSetAsync(key, EntityToJson(value!), expirationTime);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        return value;
    }

    public Task<T[]> GetAll(int[] ids) => GetAll([.. ids.Select(e => e.ToString())]);

    public async Task<T[]> GetAll(string[] ids)
    {
        RedisKey[] keys = [.. ids.Select(e => (RedisKey)KeyFrom(e))];
        RedisValue[] results = await database.StringGetAsync(keys);

        return [..results
            .Where(e => !e.IsNullOrEmpty)
            .Select(e => JsonToEntity(e))!];
    }

    public async Task Remove(T value, bool ifEquals)
    {
        var key = KeyOf(value);

        if (ifEquals)
        {
            string luaScript = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";

            await database.ScriptEvaluateAsync(
                luaScript,
                keys: [(RedisKey)key],
                values: [(RedisValue)EntityToJson(value)]
            );
        }
        else
        {
            await database.KeyDeleteAsync(key);
        }
    }

    public Task RemoveById(int id) => RemoveById(id.ToString());

    public async Task RemoveById(string id)
    {
        var key = KeyFrom(id);
        await database.KeyDeleteAsync(key);
    }

    public async Task Set(T value)
    {
        var key = KeyOf(value);
        await database.StringSetAsync(key, EntityToJson(value), expirationTime);
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

    public async Task<bool> CreateOrRenew(T value)
    {
        var key = KeyOf(value);
        var jsonValue = EntityToJson(value);

        string luaScript = @"
        local currentValue = redis.call('get', KEYS[1])
        if not currentValue or currentValue == ARGV[1] then
            redis.call('set', KEYS[1], ARGV[1], 'EX', ARGV[2])
            return 1
        else
            return 0
        end";

        var result = await database.ScriptEvaluateAsync(
            luaScript,
            keys: [(RedisKey)key],
            values: [(RedisValue)jsonValue, (int)expirationTime.TotalSeconds]
        );

        return (int)result == 1;
    }
}
