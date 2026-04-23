using HashidsNet;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Entities.Security;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Core.Security;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Services
{
    public class AccountService(
        IUserRepository userRepository,
        IUserSessionRepository userSessionRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork,
        IHashids hashids,
        UserContext userContext) : IAccountService
    {
        public async Task AnonymizeAccount(AccountDeletionDto accountDeletionDto, CancellationToken ct)
        {
            var user = await CurrentAuthenticatedUser(accountDeletionDto.Password, ct);

            await userSessionRepository.ExecuteDeleteAllSessions(user.Id, ct: ct);

            var oldUser = user.Clone();
            user.Name = user.Username = $"Anonymized {user.Id}";
            user.Avatar = string.Empty;
            user.PasswordHash = string.Empty;
            user.IsActive = false;

            await unitOfWork.CommitAsync(ct);
            await userRepository.InvalidateCache(oldUser);
        }

        public async Task ChangePassword(PasswordChangeDto passwordChange, CancellationToken ct)
        {
            var user = await CurrentAuthenticatedUser(passwordChange.CurrentPassword, ct);
            user.PasswordHash = passwordService.CalculeHash(passwordChange.NewPassword);
            
            await userRepository.InvalidateCache(user);
            await unitOfWork.CommitAsync(ct);
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

            await userSessionRepository.Add(session, ct: ct);
            await unitOfWork.CommitAsync(ct);

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

            await userRepository.Add(user, ct: ct);

            var session = new UserSession
            {
                User = user,
                UserAgent = request.UserAgent
            };

            await userSessionRepository.Add(session, ct: ct);
            await unitOfWork.CommitAsync(ct);

            return session.ToAuthResponse();
        }

        private async Task<User> CurrentAuthenticatedUser(string password, CancellationToken ct)
        {
            var user = (await userRepository.FindById(userContext.UserId, useCache: false, ct: ct));

            if (user == null || !passwordService.VerifyPassword(password, user.PasswordHash))
                throw new ForbiddenException("Senha incorreta");

            return user;
        }
    }
}
