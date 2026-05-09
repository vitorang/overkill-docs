using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Collections;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Repositories;

internal sealed class DocumentRepository(AppDbContext context, IObjectCache<DocumentCollection> documentCache) : IDocumentRepository
{
    public async Task Add(Document document, CancellationToken ct)
    {
        await context.Documents.AddAsync(document, ct);
    }

    public async Task ExecuteDelete(int documentId, CancellationToken ct)
    {
        await context.Documents.Where(e => e.Id == documentId).ExecuteDeleteAsync(ct);
    }

    public async Task<Document?> GetById(int documentId, CancellationToken ct)
    {
        return await context.Documents.FirstOrDefaultAsync(e => e.Id == documentId, ct);
    }

    public async Task<Document[]> List(CancellationToken ct)
    {
        async Task<DocumentCollection> fetchFromDb()
            => new(await context.Documents.ToArrayAsync(ct));

        return (await documentCache.Get(string.Empty, fetchFromDb!))!.Documents;
    }

    public async Task InvalidateCache()
    {
        await documentCache.RemoveById(string.Empty);
    }
}
