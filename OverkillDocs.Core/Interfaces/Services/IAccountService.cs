using OverkillDocs.Core.DTOs.Account;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AuthResponseDto> Login(AuthRequestDto request, CancellationToken ct);
        Task Logout(string? sessionHashId, CancellationToken ct);
        Task<AuthResponseDto> Register(AuthRequestDto request, CancellationToken ct);
        Task<ImmutableArray<UserSessionDto>> ListSessions(CancellationToken ct);
    }
}
