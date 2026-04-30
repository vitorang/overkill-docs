using Bogus;
using OverkillDocs.Core.DTOs.Account;

namespace OverkillDocs.Tests.Common.Fakers.DTOs.Account;

public sealed class AccountDeletionDtoFaker : Faker<AccountDeletionDto>
{
    public AccountDeletionDtoFaker()
    {
        CustomInstantiator(f => new AccountDeletionDto(f.Internet.Password()));
    }
}
