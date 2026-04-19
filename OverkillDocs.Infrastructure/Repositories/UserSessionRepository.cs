using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Security;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using System.Collections.Immutable;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserSessionRepository(AppDbContext context, IObjectCache<UserIdentity> userIdentityCache) : IUserSessionRepository
    {
        public async Task Add(UserSession userSession, CancellationToken ct)
        {
            await context.UserSessions.AddAsync(userSession, ct);
        }

        public async Task ExecuteDeleteAllSessions(int userId, CancellationToken ct)
        {
            var query = context.UserSessions.Where(e => e.UserId == userId);

            var identities = await query
                .Select(e => new UserIdentity(e.UserId, e.User.Name, e.Token))
                .ToArrayAsync(ct);

            await query.ExecuteDeleteAsync(ct);
            await userIdentityCache.RemoveAll(identities);
        }

        public async Task ExecuteDelete(string sessionToken, CancellationToken ct)
        {
            await userIdentityCache.RemoveById(sessionToken);
            await context.UserSessions.Where(e => e.Token == sessionToken).ExecuteDeleteAsync(ct);
        }

        public async Task<UserIdentity?> FindIdentityByToken(string token, CancellationToken ct)
        {
            Task<UserIdentity?> fetchFromDb() => context.UserSessions
                .Where(e => e.Token == token)
                .Select(e => new UserIdentity(e.UserId, e.User.Name, e.Token))
                .FirstOrDefaultAsync(ct);

            return await userIdentityCache.Get(token, fetchFromDb);
        }

        public async Task<UserSession?> GetById(int sessionId, CancellationToken ct)
        {
            return await context.UserSessions.Where(e => e.Id == sessionId).FirstOrDefaultAsync(ct);
        }

        public async Task<ImmutableArray<UserSession>> List(int userId, CancellationToken ct)
        {
            var sessions = context.UserSessions.Where(e => e.UserId == userId);
            return [.. (await sessions.ToArrayAsync(ct))];
        }
    }
}
