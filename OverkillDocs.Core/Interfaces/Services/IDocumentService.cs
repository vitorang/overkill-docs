using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.DTOs.Shared;

namespace OverkillDocs.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<DocumentDto> Create(DocumentDto documentDto, CancellationToken ct);
    Task<DocumentDto> Update(DocumentDto documentDto, CancellationToken ct);
    Task<DocumentDto> Get(string hashId, CancellationToken ct);
    Task<SearchResultDto<DocumentDto>> Search(string search, int page, CancellationToken ct);
    Task Delete(string hashId, CancellationToken ct);
}
