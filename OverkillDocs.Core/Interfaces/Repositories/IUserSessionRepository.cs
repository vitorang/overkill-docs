using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserContext.UserIdentity?> FindIdentityByTokenAsync(string token, CancellationToken ct);
        Task AddAsync(UserSession userSession, CancellationToken ct);
        Task DeleteAsync(string sessionToken, CancellationToken ct);
    }
}
