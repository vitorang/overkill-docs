using HashidsNet;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.CachedResults;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Repositories;

internal sealed class DocumentRepository(
    AppDbContext context,
    IObjectCache<DocumentSummariesResult> documentSummaryCache,
    IObjectCache<DocumentFragmentHashIdsResult> fragmentIdsCache,
    IHashids hashids) : IDocumentRepository
{
    public void Add(Document document)
    {
        context.Documents.Add(document);
        CacheMarkAsInvalid();
    }

    public async Task<int> ExecuteDelete(int documentId, CancellationToken ct)
    {
        var fragmentIds = await context.DocumentFragments
            .Where(e => e.DocumentId == documentId)
            .Select(e => e.Id)
            .ToArrayAsync(ct);

        var rowsAffected = await context.Documents
            .Where(e => e.Id == documentId)
            .ExecuteDeleteAsync(ct);

        if (rowsAffected > 0)
            await InvalidateCache(documentId);

        return rowsAffected;
    }

    public async Task<Document?> GetByIdReadOnly(int documentId, CancellationToken ct)
    {
        return await context.Documents
            .Include(d => d.Fragments)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == documentId, ct);
    }

    public async Task<Document?> GetByIdForUpdate(int documentId, CancellationToken ct)
    {
        var document = await context.Documents
            .Include(d => d.Fragments)
            .FirstOrDefaultAsync(e => e.Id == documentId, ct);

        if (document != null)
            CacheMarkAsInvalid(document.Id);

        return document;
    }

    public async Task<DocumentSummaryDto[]> List(CancellationToken ct)
    {
        async Task<DocumentSummariesResult> fetchFromDb()
            => new(await context.Documents
            .Select(e => new DocumentSummaryDto(
                e.Title,
                hashids.Encode(e.Id),
                e.Type
            ))
            .ToArrayAsync(ct));

        return (await documentSummaryCache.Get(DocumentSummariesResult.DefaultId, fetchFromDb!))!.Documents;
    }

    public void CacheMarkAsInvalid(int? documentId = null)
    {
        documentSummaryCache.MarkAsInvalid(DocumentSummariesResult.DefaultId);
        if (documentId != null)
            fragmentIdsCache.MarkAsInvalid((int)documentId);
    }

    public async Task InvalidateCache(int? documentId = null)
    {
        await documentSummaryCache.RemoveById(DocumentSummariesResult.DefaultId);
        if (documentId != null)
            await fragmentIdsCache.RemoveById((int)documentId);
    }

    public async Task<int?> GetDocumentIdByFragmentId(int fragmentId, CancellationToken ct)
    {
        return await context.DocumentFragments
            .Where(e => e.Id == fragmentId)
            .Select(e => e.DocumentId)
            .FirstOrDefaultAsync(ct);
    }
}
