using OverkillDocs.Core.Entities.Document;

namespace OverkillDocs.Core.Interfaces.Repositories;

public interface IDocumentFragmentRepository
{
    Task<bool> Lock(DocumentFragmentLock fragmentLock);
    Task Unlock(DocumentFragmentLock fragment);
    Task<DocumentFragmentLock?> GetLocked(int fragmentId);

    Task<DocumentFragment?> GetById(int fragmentId, CancellationToken ct);
    Task Add(DocumentFragment fragment, CancellationToken ct);
    Task ExecuteDelete(int[] fragmentIds, CancellationToken ct);
    Task<int> ExecuteUpdateContent(int fragmentId, string content, CancellationToken ct);
}
