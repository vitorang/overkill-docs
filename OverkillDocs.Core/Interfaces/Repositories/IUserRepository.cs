using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Interfaces.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Task<User?> FindById(int id, bool useCache, CancellationToken ct);
    Task<User?> FindByUsername(string username, CancellationToken ct);
    Task InvalidateCache(User user);
}
