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
    IObjectCache<DocumentListResult> documentCache,
    IDocumentFragmentRepository fragmentRepository,
    IHashids hashids) : IDocumentRepository
{
    public async Task Add(Document document, CancellationToken ct)
    {
        await context.Documents.AddAsync(document, ct);
    }

    public async Task ExecuteDelete(int documentId, CancellationToken ct)
    {
        var fragmentIds = await context.DocumentFragments
            .Where(e => e.DocumentId == documentId)
            .Select(e => e.Id)
            .ToArrayAsync(ct);

        await fragmentRepository.ExecuteDelete(fragmentIds, ct);
        await context.Documents.Where(e => e.Id == documentId).ExecuteDeleteAsync(ct);
    }

    public async Task<Document?> GetById(int documentId, CancellationToken ct)
    {
        return await context.Documents.FirstOrDefaultAsync(e => e.Id == documentId, ct);
    }

    public async Task<DocumentSummaryDto[]> List(CancellationToken ct)
    {
        async Task<DocumentListResult> fetchFromDb()
            => new(await context.Documents
            .Select(e => new DocumentSummaryDto(
                e.Title,
                hashids.Encode(e.Id),
                e.Type
            ))
            .ToArrayAsync(ct));

        return (await documentCache.Get(string.Empty, fetchFromDb!))!.Documents;
    }

    public async Task InvalidateCache()
    {
        await documentCache.RemoveById(string.Empty);
    }
}
