using OverkillDocs.Infrastructure.Interfaces;
using StackExchange.Redis;
using System.Collections.Immutable;
using System.Text.Json;

namespace OverkillDocs.Infrastructure.Cache.Redis;

internal sealed class RedisListCache<T>(IConnectionMultiplexer redis) : ListCache<T>, IListCache<T>
{
    private readonly IDatabase database = redis.GetDatabase();
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };


    public async Task Append(T value)
    {
        var transaction = database.CreateTransaction();
        _ = transaction.ListRightPushAsync(key, ToJsonString(value));
        _ = transaction.ListTrimAsync(key, -sizeLimit, -1);
        _ = transaction.KeyExpireAsync(key, expirationTime);
        await transaction.ExecuteAsync();
    }

    public async Task<ImmutableArray<T>> Get()
    {
        var listRange = database.ListRangeAsync(key);
        var keyExpire = database.KeyExpireAsync(key, expirationTime);
        await Task.WhenAll(listRange, keyExpire);

        var result = await listRange;
        if (result.Length == 0)
            return [];

        return [.. result.Select(value => ToEntity(value))
            .Where(value => value != null)
            .Cast<T>()];
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
}
