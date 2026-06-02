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
    public async Task<DocumentSummaryDto> Create(DocumentCreationDto documentDto, CancellationToken ct)
    {
        var document = new Document
        {
            Title = documentDto.Title,
            Type = documentDto.Type,
        };

        documentRepository.Add(document);
        await unitOfWork.CommitAsync(ct);
        await documentRepository.InvalidateCache();

        return document.ToSummaryDto(hashids);
    }

    public async Task Delete(string hashId, CancellationToken ct)
    {
        var documentId = hashids.Decode(hashId).First();

        var locks = await fragmentRepository.GetActiveLocksFromDocument(documentId, ct);
        if (locks.Length != 0)
            throw new ConflictException("Não pode excluir um documento com alguém editando");

        var rowsAffected = await documentRepository.ExecuteDelete(documentId, ct);
        if (rowsAffected == 0)
            throw new NotFoundException("Documento não encontrado");

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

    public async Task<DocumentFragmentDto> CreateFragment(DocumentFragmentCreationDto creationDto, CancellationToken ct)
    {
        int documentId = hashids.Decode(creationDto.DocumentHashId).First();
        var document = await documentRepository.GetById(documentId, ct);
        if (document == null)
            throw new NotFoundException("Documento não encontrado");

        var fragments = document.Fragments.OrderBy(e => e.Order).ToList();
        int afterId = creationDto.InsertAfterHashId == null ? -1 : hashids.Decode(creationDto.InsertAfterHashId).First();
        int afterIndex = fragments.FindIndex(e => e.Id == afterId);
        double order;

        if (fragments.Count == 0)
            order = 0;
        else if (creationDto.InsertAfterHashId == null)
            order = fragments.First().Order - 1000;
        else if (afterIndex == -1)
            throw new NotFoundException("Fragmento anterior não encontrado");
        else if (afterIndex == fragments.Count - 1)
            order = fragments.Last().Order + 1000;
        else
            order = (fragments[afterIndex].Order + fragments[afterIndex + 1].Order) / 2;

        document.UpdatedAt = DateTime.UtcNow;
        DocumentFragment fragment = new()
        {
            Id = 0,
            Content = creationDto.GetContent(),
            Order = order,
            Type = creationDto.Type,
            Document = document
        };

        fragmentRepository.Add(fragment);
        await unitOfWork.CommitAsync(ct);
        await fragmentRepository.InvalidateCacheByDocumentId(fragment.DocumentId);

        return fragment.ToDto(hashids);
    }

    public async Task UpdateFragment(DocumentFragmentDto fragmentDto, CancellationToken ct)
    {
        string userHashId = hashids.Encode(userContext.UserId);
        int fragmentId = hashids.Decode(fragmentDto.HashId).First();
        var fragmentLock = await fragmentRepository.GetLock(fragmentId);

        if (fragmentLock?.UserHashId != userHashId)
            throw new ConflictException("Usuário não está com posse do fragmento");

        fragmentDto.UpdatedAt = DateTime.UtcNow;
        int rowsAffected = await fragmentRepository.ExecuteUpdateContent(fragmentId, fragmentDto.GetContent(), fragmentDto.UpdatedAt, ct);
        if (rowsAffected == 0)
            throw new NotFoundException("Fragmento não encontrado");
    }

    public async Task DeleteFragment(string fragmentHashId, CancellationToken ct)
    {
        int fragmentId = hashids.Decode(fragmentHashId).First();
        var fragment = await fragmentRepository.GetById(fragmentId, ct, includeDocument: true);
        if (fragment == null)
            throw new NotFoundException("Fragmento não encontrado");

        if ((await fragmentRepository.GetLock(fragmentId)) != null)
            throw new ConflictException("Fragmento em uso não pode ser excluído");

        fragment.Document.UpdatedAt = DateTime.UtcNow;
        fragmentRepository.Remove(fragment);
        await unitOfWork.CommitAsync(ct);
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
