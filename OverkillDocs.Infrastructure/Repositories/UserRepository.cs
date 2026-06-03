using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;

namespace OverkillDocs.Infrastructure.Repositories;

internal sealed class UserRepository(AppDbContext context, IObjectCache<User> userCache) : IUserRepository
{
    public void Add(User user)
    {
        context.Users.Add(user);
    }

    public async Task<User?> FindByUsername(string username, CancellationToken ct)
    {
        return await context.Users.FirstOrDefaultAsync(e => e.Username == username && e.IsActive, ct);
    }

    public async Task<User?> GetByIdForUpdate(int id, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(e => e.Id == id && e.IsActive, ct);
        if (user != null)
            userCache.MarkAsInvalid(user);

        return user;
    }

    public async Task<User?> GetByIdReadOnly(int id, CancellationToken ct)
    {
        Task<User?> fetchFromDb() => context.Users.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive, ct);

        return await userCache.Get(id, fetchFromDb);
    }
}
