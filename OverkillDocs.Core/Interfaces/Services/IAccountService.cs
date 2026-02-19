using OverkillDocs.Core.DTOs.Account;

namespace OverkillDocs.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AuthResponseDto> LoginAsync(AuthRequestDto request, CancellationToken ct);
        Task LogoutAsync(CancellationToken ct);
        Task<AuthResponseDto> RegisterAsync(AuthRequestDto request, CancellationToken ct);
    }
}
