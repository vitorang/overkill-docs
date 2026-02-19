using OverkillDocs.Core.Entities;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> FindByTokenAsync(string token, CancellationToken ct);
        Task AddAsync(UserSession userSession, CancellationToken ct);
        Task DeleteAsync(string sessionToken, CancellationToken ct);
    }
}
