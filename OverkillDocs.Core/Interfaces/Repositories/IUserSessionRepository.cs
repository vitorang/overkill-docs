using OverkillDocs.Core.Entities;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> FindByTokenAsync(string token, CancellationToken ct = default);
        Task AddAsync(UserSession userSession);
    }
}
