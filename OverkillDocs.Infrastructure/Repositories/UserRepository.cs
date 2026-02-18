using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Interfaces.Repositories;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task AddAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
