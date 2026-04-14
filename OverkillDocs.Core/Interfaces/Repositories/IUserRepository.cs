using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task Add(User user, CancellationToken ct);
        Task<User?> FindById(int id, CancellationToken ct);
        Task<User?> FindByUsername(string username, CancellationToken ct);
    }
}
