using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OverkillDocs.Core.Constants;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Repositories;

internal sealed class DocumentRepository(AppDbContext context, IObjectCache<DocumentSearchResult> documentSearchCache) : IDocumentRepository
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

    public async Task<DocumentSearchResult> Search(string text, int page, CancellationToken ct)
    {
        async Task<DocumentSearchResult> fetchFromDb()
        {
            var skip = (page - 1) * DocumentConstants.PageSize;

            var query = text.IsNullOrEmpty()
                ? context.Documents.AsQueryable()
                : context.Documents.Where(e => e.Title.Contains(text));

            var total = await query.CountAsync(ct);

            return new(
                Text: text,
                Page: page,
                Total: total,
                HasMore: skip + DocumentConstants.PageSize < total,
                Items: await query.ToArrayAsync(ct));
        }

        var emptySearch = new DocumentSearchResult(Text: text, Page: page, Total: 0, HasMore: false, Items: []);
        var id = documentSearchCache.IdFrom(emptySearch);
        return (await documentSearchCache.Get(id, fetchFromDb!))!;
    }

    public async Task InvalidateCache()
    {
        await documentSearchCache.Clear();
    }
}
