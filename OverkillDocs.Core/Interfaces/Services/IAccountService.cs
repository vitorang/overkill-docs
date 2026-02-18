using OverkillDocs.Core.DTOs.Account;

namespace OverkillDocs.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AuthResponseDto> LoginAsync(AuthRequestDto request);
        Task LogoutAsync();
        Task<AuthResponseDto> RegisterAsync(AuthRequestDto request);
    }
}
