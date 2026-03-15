using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserSessionRepository(AppDbContext context, IAppCache<UserIdentity> userIdentityCache) : IUserSessionRepository
    {
        public async Task AddAsync(UserSession userSession, CancellationToken ct)
        {
            await context.UserSessions.AddAsync(userSession, ct);
        }

        public async Task DeleteAsync(string sessionToken, CancellationToken ct)
        {
            await userIdentityCache.RemoveById(sessionToken, ct);

            var session = await context.UserSessions.FirstOrDefaultAsync(e => e.Token == sessionToken, ct);
            if (session != null)
                context.UserSessions.Remove(session);
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
