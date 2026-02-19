using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserSessionRepository(AppDbContext context) : IUserSessionRepository
    {
        public async Task AddAsync(UserSession userSession, CancellationToken ct)
        {
            await context.UserSessions.AddAsync(userSession, ct);
        }

        public async Task DeleteAsync(string sessionToken, CancellationToken ct)
        {
            var session = await context.UserSessions.FirstOrDefaultAsync(e => e.Token == sessionToken, ct);
            if (session != null)
                context.UserSessions.Remove(session);   
        }

        public async Task<UserSession?> FindByTokenAsync(string token, CancellationToken ct)
        {
            return await context.UserSessions
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Token == token, ct);
        }
    }
}
