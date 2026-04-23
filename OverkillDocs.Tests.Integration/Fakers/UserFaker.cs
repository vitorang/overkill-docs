using Bogus;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Tests.Integration.Helpers;
using System.Text.RegularExpressions;

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
                Name = f.Name.FirstName(),
                PasswordHash = string.Empty
            });
        }
    }
}
