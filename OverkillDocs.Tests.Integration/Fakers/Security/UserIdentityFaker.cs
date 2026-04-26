using Bogus;
using OverkillDocs.Core.Entities.Identity;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Tests.Integration.Fakers.Security;

public class UserIdentityFaker : Faker<UserIdentity>
{
    public UserIdentityFaker(User user, string sessionToken)
    {
        CustomInstantiator(f => new UserIdentity(
            UserId: user.Id,
            Username: user.Username,
            Token: sessionToken
        ));
    }
}
