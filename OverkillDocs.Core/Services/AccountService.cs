using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Entities.Security;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Core.Services
{
    public class AccountService(
        IUserRepository userRepository,
        IUserSessionRepository userSessionRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork,
        UserContext userContext) : IAccountService
    {
        public async Task<AuthResponseDto> LoginAsync(AuthRequestDto request, CancellationToken ct)
        {
            var notFound = new NotFoundException("Usuário ou senha incorretos");

            var user = await userRepository.FindByUsernameAsync(request.Username, ct: ct) ?? throw notFound;
            if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
                throw notFound;

            var session = new UserSession {
                User = user,
                UserAgent = request.UserAgent
            };

            await userSessionRepository.AddAsync(session, ct: ct);
            await unitOfWork.CommitAsync(ct);

            return session.ToAuthResponse();
        }

        public async Task LogoutAsync(CancellationToken ct)
        {
            if (string.IsNullOrEmpty(userContext.Token))
                return;

            await userSessionRepository.ExecuteDeleteAsync(userContext.Token, ct);
        }

        public async Task<AuthResponseDto> RegisterAsync(AuthRequestDto request, CancellationToken ct)
        {
            var userExists = await userRepository.FindByUsernameAsync(request.Username, ct: ct) != null;
            if (userExists)
                throw new ConflictException("Nome de usuário está em uso");

            var user = new User
            {
                Name = request.Username,
                Username = request.Username,
                PasswordHash = passwordService.CalculeHash(request.Password)
            };

            await userRepository.AddAsync(user, ct: ct);

            var session = new UserSession
            {
                User = user,
                UserAgent = request.UserAgent
            };

            await userSessionRepository.AddAsync(session, ct: ct);
            await unitOfWork.CommitAsync(ct);

            return session.ToAuthResponse();
        }
    }
}
