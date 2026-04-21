using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context, IObjectCache<User> userCache) : IUserRepository
    {
        public async Task Add(User user, CancellationToken ct)
        {
            await context.Users.AddAsync(user, ct);
        }

        public async Task<User?> FindById(int id, bool useCache, CancellationToken ct)
        {
            Task<User?> fetchFromDb() => context.Users.FirstOrDefaultAsync(e => e.Id == id && e.IsActive, ct);
            return await (useCache ? userCache.Get(id, fetchFromDb) : fetchFromDb());
        }

        public async Task<User?> FindByUsername(string username, CancellationToken ct)
        {
            return await context.Users.FirstOrDefaultAsync(e => e.Username == username && e.IsActive, ct);
        }

        public async Task InvalidateCache(User user)
        {
            await userCache.Remove(user);
        }
    }
}
