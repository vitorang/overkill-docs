using HashidsNet;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Core.Services;

internal sealed class UserService(
    IUserRepository userRepository,
    UserContext userContext,
    IHashids hashids,
    IUnitOfWork unitOfWork) : IUserService
{
    public async Task<SimpleUserDto> GetByHashId(string hashId, CancellationToken ct)
    {
        var ids = hashids.Decode(hashId);
        if (ids.Length == 0)
            throw new NotFoundException($"HashId '{hashId}' inválido.");
        var id = ids[0];

        var user = await userRepository.GetByIdReadOnly(id, ct: ct);
        if (user == null)
            throw new NotFoundException($"Usuário id={id} não encontrado.");

        return user.ToSimpleDto(hashids);
    }

    public async Task<SimpleUserDto> GetCurrent(CancellationToken ct)
    {
        var user = await GetCurrentUserReadOnly(ct);
        return user.ToSimpleDto(hashids);
    }

    public async Task<ProfileDto> GetProfile(CancellationToken ct)
    {
        var user = await GetCurrentUserReadOnly(ct);
        return user.ToProfileDto(hashids);
    }

    public async Task<ProfileDto> UpdateProfile(ProfileDto profileDto, CancellationToken ct)
    {
        var user = await GetCurrentUserForUpdate(ct);
        if (user.Username != profileDto.Username)
            throw new ForbiddenException($"Alteração de perfil de {user.Username} por {profileDto.Username} não permitida.");

        user.Name = profileDto.Name;
        user.Avatar = profileDto.Avatar;

        await unitOfWork.CommitAsync(ct);
        return user.ToProfileDto(hashids);
    }

    private async Task<User> GetCurrentUserReadOnly(CancellationToken ct)
    {
        var user = await userRepository.GetByIdReadOnly(userContext.UserId, ct);
        if (user == null)
            throw new NotFoundException($"Usuário id={userContext.UserId} não encontrado.");

        return user;
    }

    private async Task<User> GetCurrentUserForUpdate(CancellationToken ct)
    {
        var user = await userRepository.GetByIdForUpdate(userContext.UserId, ct);
        if (user == null)
            throw new NotFoundException($"Usuário id={userContext.UserId} não encontrado.");

        return user;
    }
}
