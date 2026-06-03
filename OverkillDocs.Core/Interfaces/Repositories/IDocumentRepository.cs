using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Document;

namespace OverkillDocs.Core.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<DocumentSummaryDto[]> List(CancellationToken ct);
    Task<Document?> GetByIdReadOnly(int documentId, CancellationToken ct);
    Task<Document?> GetByIdForUpdate(int documentId, CancellationToken ct);
    void Add(Document document);
    Task<int> ExecuteDelete(int documentId, CancellationToken ct);
    Task<int?> GetDocumentIdByFragmentId(int fragmentId, CancellationToken ct);
}
