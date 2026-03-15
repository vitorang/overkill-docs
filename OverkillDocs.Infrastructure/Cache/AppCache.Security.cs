using OverkillDocs.Core.Interfaces;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Cache
{
    public partial class AppCache
    {
        Task<UserIdentity?> IAppCache<UserIdentity>.Get(string id) => GetValue<UserIdentity>(id);
        Task IAppCache<UserIdentity>.Set(UserIdentity value) => SetValue(value);
        Task IAppCache<UserIdentity>.Remove(UserIdentity value) => RemoveValue(value);
    }
}
