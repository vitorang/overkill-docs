using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Extensions;

public static class IdentityExtensions
{
    public static AuthResponseDto ToAuthResponse(this UserSession session)
    {
        return new AuthResponseDto(
            Token: session.Token
        );
    }
}
