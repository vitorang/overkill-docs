using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Core.Services
{
    public class AccountService(
        IUserRepository userRepository,
        IUserSessionRepository userSessionRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork) : IAccountService
    {
        public async Task<AuthResponseDto> LoginAsync(AuthRequestDto request)
        {
            var notFound = new NotFoundException("Usuário não encontrado ou senha incorreta.");

            var user = await userRepository.FindByUsernameAsync(request.Username) ?? throw notFound;
            if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
                throw notFound;

            var session = new UserSession {
                User = user,
                UserAgent = request.UserAgent
            };

            await userSessionRepository.AddAsync(session);
            await unitOfWork.CommitAsync();

            return session.ToAuthResponse();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponseDto> RegisterAsync(AuthRequestDto request)
        {
            var userExists = await userRepository.FindByUsernameAsync(request.Username) != null;
            if (userExists)
                throw new ConflictException("Nome de usuário está em uso");

            var user = new User
            {
                Name = request.Username,
                Username = request.Username,
                PasswordHash = passwordService.CalculeHash(request.Password)
            };

            await userRepository.AddAsync(user);

            var session = new UserSession
            {
                User = user,
                UserAgent = request.UserAgent
            };

            await userSessionRepository.AddAsync(session);
            await unitOfWork.CommitAsync();

            return session.ToAuthResponse();
        }
    }
}
