using HashidsNet;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Extensions;

public static class UserExtensions
{
    public static SimpleUserDto ToSimpleDto(this User user, IHashids hashids)
    {
        return new SimpleUserDto
        (
            HashId: hashids.Encode(user.Id),
            Name: user.Name,
            Avatar: user.Avatar
        );
    }

    public static ProfileDto ToProfileDto(this User user, IHashids hashids)
    {
        return new ProfileDto
        (
            Username: user.Username,
            Name: user.Name,
            Avatar: user.Avatar,
            HashId: hashids.Encode(user.Id)
        );
    }

    public static User Clone(this User user)
    {
        return new User
        {
            Username = user.Username,
            Avatar = user.Avatar,
            Name = user.Name,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            Id = user.Id,
            IsActive = user.IsActive,
            UpdatedAt = user.UpdatedAt
        };
    }
}
