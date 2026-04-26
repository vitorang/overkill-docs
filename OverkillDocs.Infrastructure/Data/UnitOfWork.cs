using OverkillDocs.Core.Interfaces;

namespace OverkillDocs.Infrastructure.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }
}
