using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken ct);
        Task<User?> FindByIdAsync(int id, CancellationToken ct);
        Task<User?> FindByUsernameAsync(string username, CancellationToken ct);
    }
}
