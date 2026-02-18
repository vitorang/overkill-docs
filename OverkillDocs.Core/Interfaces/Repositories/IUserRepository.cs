using OverkillDocs.Core.Entities;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}
