using HashidsNet;
using Microsoft.Extensions.Logging;
using OverkillDocs.Core.Constants;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Core.Security;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Services;

internal sealed class AccountService(
    IUserRepository userRepository,
    IUserSessionRepository userSessionRepository,
    IPasswordService passwordService,
    IUnitOfWork unitOfWork,
    IHashids hashids,
    ILogger<DocumentService> logger,
    UserContext userContext) : IAccountService
{
    public async Task AnonymizeAccount(AccountDeletionDto accountDeletionDto, CancellationToken ct)
    {
        var user = await CurrentAuthenticatedUser(accountDeletionDto.Password, ct);

        await userSessionRepository.ExecuteDeleteAllSessions(user.Id, ct: ct);

        user.Name = user.Username = $"{AccountConstants.AnonymizedPrefix}{user.Id}";
        user.Avatar = string.Empty;
        user.PasswordHash = string.Empty;
        user.IsActive = false;

        await unitOfWork.CommitAsync(ct);
        logger.LogInformation("Usuário {UserId} anonimizou a conta", user.Id);
    }

    public async Task ChangePassword(PasswordChangeDto passwordChange, CancellationToken ct)
    {
        var user = await CurrentAuthenticatedUser(passwordChange.CurrentPassword, ct);
        user.PasswordHash = passwordService.CalculeHash(passwordChange.NewPassword);

        await unitOfWork.CommitAsync(ct);
        logger.LogInformation("Usuário {UserId} alterou a senha", user.Id);
    }

    public async Task<ImmutableArray<UserSessionDto>> ListSessions(CancellationToken ct)
    {
        var sessions = (await userSessionRepository.List(userContext.UserId, ct: ct))
            .Select(e => e.ToDto(userContext.Token, hashids));

        return [.. sessions];
    }

    public async Task<AuthResponseDto> Login(AuthRequestDto request, CancellationToken ct)
    {
        var notFound = new NotFoundException("Usuário ou senha incorretos");

        var user = await userRepository.FindByUsername(request.Username, ct: ct) ?? throw notFound;
        if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
            throw notFound;

        var session = new UserSession
        {
            User = user,
            UserAgent = request.UserAgent
        };

        userSessionRepository.Add(session);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Usuário {UserId} fez login", user.Id);
        return session.ToAuthResponse();
    }

    public async Task Logout(string? sessionHashId, CancellationToken ct)
    {
        string token = userContext.Token;

        if (sessionHashId != null)
        {
            int sessionId = hashids.Decode(sessionHashId).First();
            var session = await userSessionRepository.GetById(sessionId, ct);

            if (session == null)
                throw new NotFoundException($"Sessão não encontrada.");

            if (session.UserId != userContext.UserId)
                throw new ForbiddenException($"Remoção de sessão não permitida.");

            token = session.Token;
        }

        if (string.IsNullOrEmpty(token))
            return;

        await userSessionRepository.ExecuteDelete(token, ct);
    }

    public async Task<AuthResponseDto> Register(AuthRequestDto request, CancellationToken ct)
    {
        var userExists = await userRepository.FindByUsername(request.Username, ct: ct) != null;
        if (userExists)
            throw new ConflictException("Nome de usuário está em uso");

        var user = new User
        {
            Name = request.Username,
            Username = request.Username,
            PasswordHash = passwordService.CalculeHash(request.Password)
        };

        userRepository.Add(user);

        var session = new UserSession
        {
            User = user,
            UserAgent = request.UserAgent
        };

        userSessionRepository.Add(session);
        await unitOfWork.CommitAsync(ct);
        logger.LogInformation("Usuário {UserId} criou a conta", user.Id);

        return session.ToAuthResponse();
    }

    private async Task<User> CurrentAuthenticatedUser(string password, CancellationToken ct)
    {
        var user = (await userRepository.GetByIdForUpdate(userContext.UserId, ct));

        if (user == null || !passwordService.VerifyPassword(password, user.PasswordHash))
            throw new ForbiddenException("Senha incorreta");

        return user;
    }
}
