using Microsoft.Extensions.Caching.Memory;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Cache.Memory;

internal sealed class MemoryCacheInvalidator(IMemoryCache cache) : ICacheInvalidator
{
    private readonly HashSet<string> Keys = [];

    public Task InvalidateAllScheduled()
    {
        foreach (var key in Keys)
            cache.Remove(key);

        Keys.Clear();
        return Task.CompletedTask;
    }

    public void MarkAsInvalid(string key) => Keys.Add(key);
}
