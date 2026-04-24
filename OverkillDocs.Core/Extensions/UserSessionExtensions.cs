using HashidsNet;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Extensions
{
    public static class UserSessionExtensions
    {
        public static UserSessionDto ToDto(this UserSession session, string currentSessionToken, IHashids hashids)
        {
            return new UserSessionDto(
                HashId: hashids.Encode(session.Id),
                UserAgent: session.UserAgent,
                IsCurrent: session.Token == currentSessionToken
            );
        }
    }
}
