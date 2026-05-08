using OverkillDocs.Core.Entities.Document;

namespace OverkillDocs.Core.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<DocumentSearchResult> Search(string text, int page, CancellationToken ct);
    Task<Document?> GetById(int documentId, CancellationToken ct);
    Task Add(Document document, CancellationToken ct);
    Task ExecuteDelete(int documentId, CancellationToken ct);
    Task InvalidateCache();
}
