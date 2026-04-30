using Bogus;
using OverkillDocs.Core.DTOs.Account;

namespace OverkillDocs.Tests.Common.Fakers.DTOs.Account;

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
