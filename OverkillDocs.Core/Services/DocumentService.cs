using HashidsNet;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.DTOs.Document.Fragment;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Core.Services;

internal sealed class DocumentService(
    IDocumentRepository documentRepository,
    IDocumentFragmentRepository fragmentRepository,
    IUnitOfWork unitOfWork,
    IHashids hashids,
    UserContext userContext) : IDocumentService
{
    public async Task<DocumentSummaryDto> Create(DocumentSummaryDto documentDto, CancellationToken ct)
    {
        var document = new Document
        {
            Title = documentDto.Title,
            Type = documentDto.Type,
        };

        await documentRepository.Add(document, ct);
        await unitOfWork.CommitAsync(ct);
        await documentRepository.InvalidateCache(document.Id);

        return document.ToSummaryDto(hashids);
    }

    public async Task Delete(string hashId, CancellationToken ct)
    {
        var documentId = hashids.Decode(hashId).First();
        await documentRepository.ExecuteDelete(documentId, ct);
        await documentRepository.InvalidateCache(documentId);
    }

    public async Task<DocumentDetailDto> Get(string hashId, CancellationToken ct)
    {
        var documentId = hashids.Decode(hashId).First();
        var document = await documentRepository.GetById(documentId, ct);
        if (document == null)
            throw new NotFoundException($"Documento não encontrado");

        return document.ToDetailDto(hashids);
    }

    public async Task<DocumentSummaryDto[]> List(CancellationToken ct)
    {
        return await documentRepository.List(ct);
    }

    public async Task<DocumentSummaryDto> Update(DocumentSummaryDto documentDto, CancellationToken ct)
    {
        var documentId = hashids.Decode(documentDto.HashId).First();
        var document = await documentRepository.GetById(documentId, ct);

        if (document == null)
            throw new NotFoundException("Documento não encontrado");

        document.Title = documentDto.Title;
        document.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.CommitAsync(ct);
        await documentRepository.InvalidateCache(document.Id);
        return document.ToSummaryDto(hashids);
    }

    public async Task<DocumentFragmentDto> CreateFragment(DocumentFragmentDto fragmentDto, CancellationToken ct)
    {
        int documentId = hashids.Decode(fragmentDto.DocumentHashId).First();
        var document = await documentRepository.GetById(documentId, ct);
        if (document == null)
            throw new NotFoundException("Documento não encontrado");

        DocumentFragment fragment = new()
        {
            Id = 0,
            Content = fragmentDto.GetContent(),
            Order = fragmentDto.Order,
            Type = fragmentDto.Type,
            Document = document
        };

        await fragmentRepository.Add(fragment, ct);
        await unitOfWork.CommitAsync(ct);
        return fragment.ToDto(hashids);
    }

    public async Task UpdateFragment(DocumentFragmentDto fragmentDto, CancellationToken ct)
    {
        string userHashId = hashids.Encode(userContext.UserId);
        int fragmentId = hashids.Decode(fragmentDto.HashId).First();
        if ((await fragmentRepository.GetLock(fragmentId))?.UserHashId != userHashId)
            throw new ConflictException("Usuário não está com posse do fragmento");

        int rowsAffected = await fragmentRepository.ExecuteUpdateContent(fragmentId, fragmentDto.GetContent(), ct);
        if (rowsAffected == 0)
            throw new NotFoundException("Fragmento não encontrado");
    }

    public async Task DeleteFragment(string fragmentHashId, CancellationToken ct)
    {
        int fragmentId = hashids.Decode(fragmentHashId).First();

        if ((await fragmentRepository.GetLock(fragmentId)) != null)
            throw new ConflictException("Fragmento em uso não pode ser excluído");

        await fragmentRepository.ExecuteDelete([fragmentId], ct);
    }

    public async Task<string> GetDocumentHashIdByFragmentHashId(string fragmentHashId, CancellationToken ct)
    {
        int fragmentId = hashids.Decode(fragmentHashId).First();
        int? documentId = await documentRepository.GetDocumentIdByFragmentId(fragmentId, ct);

        if (documentId == null)
            throw new NotFoundException("Documento não encontrado");

        return hashids.Encode((int)documentId);
    }

    public async Task LockFragment(string fragmentHashId, CancellationToken ct)
    {
        DocumentFragmentLockDto fragmentLock = new(
            FragmentHashId: fragmentHashId,
            UserHashId: hashids.Encode(userContext.UserId)
        );

        var isLocked = await fragmentRepository.Lock(fragmentLock);

        if (!isLocked)
            throw new ConflictException("Fragmento está em uso por outro usuário");
    }

    public async Task UnlockFragment(string fragmentHashId, CancellationToken ct)
    {
        DocumentFragmentLockDto fragmentLock = new(
            FragmentHashId: fragmentHashId,
            UserHashId: hashids.Encode(userContext.UserId)
        );

        await fragmentRepository.Unlock(fragmentLock);
    }

    public async Task<DocumentFragmentLockDto[]> GetActiveLocks(string documentHashId, CancellationToken ct)
    {
        int documentId = hashids.Decode(documentHashId).First();
        return await fragmentRepository.GetActiveLocksFromDocument(documentId, ct);
    }
}
