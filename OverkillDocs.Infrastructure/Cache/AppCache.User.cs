using OverkillDocs.Core.DTOs.Users;
using OverkillDocs.Core.Interfaces;

namespace OverkillDocs.Infrastructure.Cache
{
    public partial class AppCache
    {
        Task<SimpleUserDto?> IAppCache<SimpleUserDto>.Get(string id) => GetValue<SimpleUserDto>(id);
        Task IAppCache<SimpleUserDto>.Set(SimpleUserDto value) => SetValue(value);
        Task IAppCache<SimpleUserDto>.Remove(SimpleUserDto value) => RemoveValue(value);
    }
}
