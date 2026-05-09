using OverkillDocs.Core.DTOs.Document;

namespace OverkillDocs.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<DocumentDto> Create(DocumentDto documentDto, CancellationToken ct);
    Task<DocumentDto> Update(DocumentDto documentDto, CancellationToken ct);
    Task<DocumentDto> Get(string hashId, CancellationToken ct);
    Task<DocumentDto[]> List(CancellationToken ct);
    Task Delete(string hashId, CancellationToken ct);
}
