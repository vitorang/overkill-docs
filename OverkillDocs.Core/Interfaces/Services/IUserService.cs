using OverkillDocs.Core.DTOs.Users;

namespace OverkillDocs.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<SimpleUserDto> GetCurrent(CancellationToken ct);
        Task<SimpleUserDto> GetByHashId(string hashId, CancellationToken ct);
    }
}
