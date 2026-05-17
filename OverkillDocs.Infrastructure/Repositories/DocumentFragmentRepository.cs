using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Repositories;

internal sealed class DocumentFragmentRepository(
    AppDbContext context,
    IObjectCache<DocumentFragmentLock> lockRepository
    ) : IDocumentFragmentRepository
{
    public async Task Add(DocumentFragment fragment, CancellationToken ct)
    {
        await context.DocumentFragments.AddAsync(fragment, ct);
    }

    public async Task ExecuteDelete(int[] fragmentIds, CancellationToken ct)
    {
        await context.DocumentFragments
            .Where(e => fragmentIds.Contains(e.Id))
            .ExecuteDeleteAsync(ct);
    }

    public async Task<int> ExecuteUpdateContent(int fragmentId, string content, CancellationToken ct)
    {
        return await context.DocumentFragments.Where(e => e.Id == fragmentId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(f => f.Content, content)
                .SetProperty(f => f.UpdatedAt, DateTime.UtcNow),
            ct);
    }

    public async Task<DocumentFragment?> GetById(int fragmentId, CancellationToken ct)
    {
        return await context.DocumentFragments
            .Where(e => e.Id == fragmentId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<DocumentFragmentLock?> GetLocked(int fragmentId)
    {
        return await lockRepository.Get(fragmentId);
    }

    public async Task<bool> Lock(DocumentFragmentLock fragmentLock)
    {
        return await lockRepository.CreateOrRenew(fragmentLock);
    }

    public async Task Unlock(DocumentFragmentLock fragmentLock)
    {
        await lockRepository.Remove(fragmentLock, ifEquals: true);
    }
}
