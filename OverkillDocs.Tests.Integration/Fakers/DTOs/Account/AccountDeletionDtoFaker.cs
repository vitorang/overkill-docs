using Bogus;
using OverkillDocs.Core.DTOs.Account;

namespace OverkillDocs.Tests.Integration.Fakers.DTOs.Account;

public sealed class AccountDeletionDtoFaker : Faker<AccountDeletionDto>
{
    public AccountDeletionDtoFaker()
    {
        CustomInstantiator(f => new AccountDeletionDto(f.Internet.Password()));
    }
}
