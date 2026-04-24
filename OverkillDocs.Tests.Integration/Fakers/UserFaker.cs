using Bogus;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Tests.Integration.Helpers;

namespace OverkillDocs.Tests.Integration.Fakers
{
    public sealed partial class UserFaker : Faker<User>
    {
        public UserFaker()
        {
            CustomInstantiator(f => new User
            {
                Id = 0,
                Username = StringHelper.SanitizeUsername(f.Internet.UserName()),
                Name = f.Name.FirstName().Truncate(15),
                PasswordHash = string.Empty
            });
        }
    }
}
