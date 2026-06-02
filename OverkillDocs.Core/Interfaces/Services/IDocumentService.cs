using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.DTOs.Document.Fragment;

namespace OverkillDocs.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<DocumentSummaryDto> Create(DocumentCreationDto documentDto, CancellationToken ct);
    Task<DocumentSummaryDto> Update(DocumentSummaryDto documentDto, CancellationToken ct);
    Task<DocumentDetailDto> Get(string hashId, CancellationToken ct);
    Task<DocumentSummaryDto[]> List(CancellationToken ct);
    Task Delete(string hashId, CancellationToken ct);

    Task<DocumentFragmentDto> CreateFragment(DocumentFragmentCreationDto fragmentDto, CancellationToken ct);
    Task UpdateFragment(DocumentFragmentDto fragmentDto, CancellationToken ct);
    Task DeleteFragment(string fragmentHashId, CancellationToken ct);
    Task<string> GetDocumentHashIdByFragmentHashId(string fragmentHashId, CancellationToken ct);

    Task LockFragment(string fragmentHashId, CancellationToken ct);
    Task UnlockFragment(string fragmentHashId, CancellationToken ct);
    Task<DocumentFragmentLockDto[]> GetActiveLocks(string documentHashId, CancellationToken ct);
}
