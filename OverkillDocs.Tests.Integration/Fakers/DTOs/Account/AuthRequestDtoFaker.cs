using Bogus;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Tests.Integration.Helpers;

namespace OverkillDocs.Tests.Integration.Fakers.DTOs.Account;

public sealed class AuthRequestDtoFaker : Faker<AuthRequestDto>
{
    public AuthRequestDtoFaker()
    {
        CustomInstantiator(f => new AuthRequestDto(
            Username: StringHelper.SanitizeUsername(f.Internet.UserName()),
            Password: f.Internet.Password(),
            UserAgent: f.Internet.UserAgent()
        ));
    }
}
