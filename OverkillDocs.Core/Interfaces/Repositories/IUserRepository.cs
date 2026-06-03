using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Interfaces.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetByIdReadOnly(int id, CancellationToken ct);
    Task<User?> GetByIdForUpdate(int id, CancellationToken ct);
    Task<User?> FindByUsername(string username, CancellationToken ct);
}
