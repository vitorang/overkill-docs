using OverkillDocs.Core.Entities;
using OverkillDocs.Core.Interfaces.Repositories;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        public Task AddAsync(UserSession userSession)
        {
            throw new NotImplementedException();
        }

        public Task<UserSession?> FindByTokenAsync(string token, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
