using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context, IAppCache<User> userCache) : IUserRepository
    {
        public async Task AddAsync(User user, CancellationToken ct)
        {
            await context.Users.AddAsync(user, ct);
        }

        public async Task<User?> FindByIdAsync(int id, CancellationToken ct)
        {
            Task<User?> fetchFromDb() => context.Users.FirstOrDefaultAsync(e => e.Id == id, ct);

            return await userCache.Get(id, ct, fetchFromDb);
        }

        public async Task<User?> FindByUsernameAsync(string username, CancellationToken ct)
        {
            return await context.Users.FirstOrDefaultAsync(e => e.Username == username, ct);
        }
    }
}
