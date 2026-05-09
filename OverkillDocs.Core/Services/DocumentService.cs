using HashidsNet;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Core.Services;

internal sealed class DocumentService(
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork,
    IHashids hashids) : IDocumentService
{
    public async Task<DocumentDto> Create(DocumentDto documentDto, CancellationToken ct)
    {
        var document = new Document
        {
            Title = documentDto.Title,
            Type = documentDto.Type,
        };

        await documentRepository.Add(document, ct);
        await unitOfWork.CommitAsync(ct);
        await documentRepository.InvalidateCache();

        return document.ToDto(hashids);
    }

    public async Task Delete(string hashId, CancellationToken ct)
    {
        var documentId = hashids.Decode(hashId).First();
        await documentRepository.ExecuteDelete(documentId, ct);
        await documentRepository.InvalidateCache();
    }

    public async Task<DocumentDto> Get(string hashId, CancellationToken ct)
    {
        var documentId = hashids.Decode(hashId).First();
        var document = await documentRepository.GetById(documentId, ct);
        if (document == null)
            throw new NotFoundException($"Documento não encontrado");

        return document.ToDto(hashids);
    }

    public async Task<DocumentDto[]> List(CancellationToken ct)
    {
        var documents = await documentRepository.List(ct);
        return [.. documents.Select(e => e.ToDto(hashids))];
    }

    public async Task<DocumentDto> Update(DocumentDto documentDto, CancellationToken ct)
    {
        var documentId = hashids.Decode(documentDto.HashId).First();
        var document = await documentRepository.GetById(documentId, ct);

        if (document == null)
            throw new NotFoundException("Documento não encontrado");

        document.Title = documentDto.Title;
        document.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.CommitAsync(ct);
        await documentRepository.InvalidateCache();

        return document.ToDto(hashids);
    }
}
