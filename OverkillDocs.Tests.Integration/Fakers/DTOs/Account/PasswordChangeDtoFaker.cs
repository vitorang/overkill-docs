using Bogus;
using OverkillDocs.Core.DTOs.Account;

namespace OverkillDocs.Tests.Integration.Fakers.DTOs.Account;

public sealed class PasswordChangeDtoFaker : Faker<PasswordChangeDto>
{
    public PasswordChangeDtoFaker()
    {
        CustomInstantiator(f => new PasswordChangeDto
        (
            f.Internet.Password(),
            f.Internet.Password()
        ));
    }
}
