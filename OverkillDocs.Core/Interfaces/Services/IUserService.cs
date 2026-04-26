using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.DTOs.User;

namespace OverkillDocs.Core.Interfaces.Services;

public interface IUserService
{
    Task<SimpleUserDto> GetCurrent(CancellationToken ct);
    Task<SimpleUserDto> GetByHashId(string hashId, CancellationToken ct);
    Task<ProfileDto> GetProfile(CancellationToken ct);
    Task<ProfileDto> UpdateProfile(ProfileDto profileDto, CancellationToken ct);
}
