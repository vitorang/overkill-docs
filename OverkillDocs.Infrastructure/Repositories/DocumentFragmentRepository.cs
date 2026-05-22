using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.CachedResults;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Repositories;

internal sealed class DocumentFragmentRepository(
    AppDbContext context,
    IObjectCache<DocumentFragmentLockDto> lockCache,
    IObjectCache<DocumentFragmentIdsResult> fragmentIdsCache
    ) : IDocumentFragmentRepository
{
    public void Add(DocumentFragment fragment)
    {
        context.DocumentFragments.Add(fragment);
    }

    public void Remove(DocumentFragment fragment)
    {
        context.DocumentFragments.Remove(fragment);
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

    public async Task<DocumentFragment?> GetById(int fragmentId, CancellationToken ct, bool includeDocument)
    {
        return await context.DocumentFragments
            .Where(e => e.Id == fragmentId)
            .Include(e => includeDocument ? e.Document : null)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<DocumentFragmentLockDto?> GetLock(int fragmentId)
    {
        return await lockCache.Get(fragmentId);
    }

    public async Task<bool> Lock(DocumentFragmentLockDto fragmentLock)
    {
        return await lockCache.CreateOrRenew(fragmentLock);
    }

    public async Task Unlock(DocumentFragmentLockDto fragmentLock)
    {
        await lockCache.Remove(fragmentLock, ifEquals: true);
    }

    public async Task<DocumentFragmentLockDto[]> GetActiveLocksFromDocument(int documentId, CancellationToken ct)
    {
        async Task<DocumentFragmentIdsResult> onCacheMiss()
        {
            return new DocumentFragmentIdsResult(
                DocumentId: documentId,
                FragmentIds: await context.DocumentFragments.Select(e => e.Id).ToArrayAsync(ct)
            );
        }

        var fragmentResult = await fragmentIdsCache.Get(documentId, onCacheMiss!);
        return await lockCache.GetAll(fragmentResult!.FragmentIds);
    }
}
