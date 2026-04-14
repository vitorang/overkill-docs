using OverkillDocs.Core.Entities.Security;
using OverkillDocs.Core.Security;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserContext.UserIdentity?> FindIdentityByToken(string token, CancellationToken ct);
        Task Add(UserSession userSession, CancellationToken ct);
        Task ExecuteDelete(string sessionToken, CancellationToken ct);
        Task<ImmutableArray<UserSession>> List(int userId, CancellationToken ct);
        Task<UserSession?> GetById(int sessionId, CancellationToken ct);
    }
}
