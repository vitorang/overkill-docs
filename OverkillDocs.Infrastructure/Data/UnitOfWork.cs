using OverkillDocs.Core.Interfaces;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Data;

internal sealed class UnitOfWork(AppDbContext context, ICacheInvalidator cacheInvalidator) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct)
    {
        var result = await context.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAllScheduled();
        return result;
    }
}
