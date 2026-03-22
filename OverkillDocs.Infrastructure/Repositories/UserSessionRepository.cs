using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Security;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserSessionRepository(AppDbContext context, IObjectCache<UserIdentity> userIdentityCache) : IUserSessionRepository
    {
        public async Task AddAsync(UserSession userSession, CancellationToken ct)
        {
            await context.UserSessions.AddAsync(userSession, ct);
        }

        public async Task ExecuteDeleteAsync(string sessionToken, CancellationToken ct)
        {
            await userIdentityCache.RemoveById(sessionToken, ct);
            await context.UserSessions.Where(e => e.Token == sessionToken).ExecuteDeleteAsync(ct);
        }

        public async Task<UserIdentity?> FindIdentityByTokenAsync(string token, CancellationToken ct)
        {
            Task<UserIdentity?> fetchFromDb() => context.UserSessions
                .Where(e => e.Token == token)
                .Select(e => new UserIdentity(e.UserId, e.User.Name, e.Token))
                .FirstOrDefaultAsync(ct);

            return await userIdentityCache.Get(token, ct, fetchFromDb);
        }
    }
}
