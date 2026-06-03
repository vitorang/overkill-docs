using HashidsNet;
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
    IObjectCache<DocumentFragmentIdsResult> fragmentIdsCache,
    IHashids hashids
    ) : IDocumentFragmentRepository
{
    public void Add(DocumentFragment fragment)
    {
        context.DocumentFragments.Add(fragment);
        fragmentIdsCache.MarkAsInvalid(fragment.DocumentId);
    }

    public void Remove(DocumentFragment fragment)
    {
        context.DocumentFragments.Remove(fragment);
        fragmentIdsCache.MarkAsInvalid(fragment.DocumentId);
    }

    public async Task<int> ExecuteUpdateContent(int fragmentId, string content, DateTime updatedAt, CancellationToken ct)
    {
        return await context.DocumentFragments.Where(e => e.Id == fragmentId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(f => f.Content, content)
                .SetProperty(f => f.UpdatedAt, updatedAt),
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
        return await lockCache.Get(hashids.Encode(fragmentId));
    }

    public async Task<bool> Lock(DocumentFragmentLockDto fragmentLock)
    {
        return await lockCache.CreateOrRenew(fragmentLock);
    }

    public async Task Unlock(DocumentFragmentLockDto fragmentLock)
    {
        await lockCache.RemoveIfEquals(fragmentLock);
    }

    public async Task<DocumentFragmentLockDto[]> GetActiveLocksFromDocument(int documentId, CancellationToken ct)
    {
        async Task<DocumentFragmentIdsResult> onCacheMiss()
        {
            var fragmentIds = await context.DocumentFragments.Select(e => e.Id).ToArrayAsync(ct);

            return new DocumentFragmentIdsResult(
                DocumentId: documentId,
                FragmentIds: fragmentIds
            );
        }

        var fragmentResult = await fragmentIdsCache.Get(documentId, onCacheMiss!);
        var hashIds = fragmentResult!.FragmentIds.Select(e => hashids.Encode(e)).ToArray();
        return await lockCache.GetAll(hashIds);
    }
}
