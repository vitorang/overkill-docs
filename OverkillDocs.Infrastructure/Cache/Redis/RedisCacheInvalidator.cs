using OverkillDocs.Infrastructure.Interfaces;
using StackExchange.Redis;

namespace OverkillDocs.Infrastructure.Cache.Redis;

internal sealed class RedisCacheInvalidator(IConnectionMultiplexer redis) : ICacheInvalidator
{
    private readonly HashSet<string> Keys = [];
    private readonly IDatabase database = redis.GetDatabase();

    public async Task InvalidateAllScheduled()
    {
        if (Keys.Count == 0) return;

        var batch = database.CreateBatch();

        var tasks = Keys
            .Select(key => batch.KeyDeleteAsync(key))
            .ToArray();

        batch.Execute();
        await Task.WhenAll(tasks);
        Keys.Clear();
    }

    public void MarkAsInvalid(string key) => Keys.Add(key);
}
