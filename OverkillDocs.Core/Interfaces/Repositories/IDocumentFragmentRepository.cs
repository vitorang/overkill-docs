using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Document;

namespace OverkillDocs.Core.Interfaces.Repositories;

public interface IDocumentFragmentRepository
{
    Task<bool> Lock(DocumentFragmentLockDto fragmentLock);
    Task Unlock(DocumentFragmentLockDto fragment);
    Task<DocumentFragmentLockDto?> GetLock(int fragmentId);

    Task<DocumentFragment?> GetById(int fragmentId, CancellationToken ct, bool includeDocument = false);
    void Add(DocumentFragment fragment);
    void Remove(DocumentFragment fragment);
    Task ExecuteDelete(int[] fragmentIds, CancellationToken ct);
    Task<int> ExecuteUpdateContent(int fragmentId, string content, CancellationToken ct);
    Task<DocumentFragmentLockDto[]> GetActiveLocksFromDocument(int documentId, CancellationToken ct);
}
