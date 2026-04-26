using OverkillDocs.Core.Attributes;

namespace OverkillDocs.Core.DTOs.Account
{
    public record ProfileDto(
        [ProfileName]
        string Name,
        string Username,
        string Avatar,
        string HashId
    );
}
