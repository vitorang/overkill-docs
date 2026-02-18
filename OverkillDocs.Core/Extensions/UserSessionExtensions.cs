using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities;

namespace OverkillDocs.Core.Extensions
{
    public static class UserSessionExtensions
    {
        public static AuthResponseDto ToAuthResponse(this UserSession session)
        {
            return new AuthResponseDto(
                Token: session.Token
            );
        }
    }
}
