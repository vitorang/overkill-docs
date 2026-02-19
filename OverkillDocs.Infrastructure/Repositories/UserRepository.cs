using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task AddAsync(User user, CancellationToken ct)
        {
            await context.Users.AddAsync(user, ct);
        }

        public async Task<User?> FindByUsernameAsync(string username, CancellationToken ct)
        {
            return await context.Users.FirstOrDefaultAsync(e => e.Username == username, ct);
        }
    }
}
