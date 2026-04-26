using Bogus;
using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Tests.Integration.Fakers.Entities.Identity;

public class UserSessionFaker : Faker<UserSession>
{
    public UserSessionFaker(User user)
    {
        CustomInstantiator(f => new UserSession
        {
            UserAgent = f.Internet.UserAgent(),
            User = user,
            UserId = user.Id,
        });
    }
}
