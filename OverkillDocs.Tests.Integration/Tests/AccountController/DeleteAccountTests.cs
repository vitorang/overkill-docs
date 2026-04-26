using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Constants;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using static OverkillDocs.Tests.Integration.Fakers.DTOs.Account.AuthRequestDtoFaker;

namespace OverkillDocs.Tests.Integration.Tests.AccountController
{
    public class DeleteAccountTests
    {
        private static readonly string url = "/api/account/delete-account";

        public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task WithCorrectPassword_AnonymizesAccountAndLogout()
            {
                var passwordService = Require<IPasswordService>();
                var accountDeletion = new AccountDeletionDtoFaker().Generate();
                var user = new UserFaker().Generate();
                user.PasswordHash = passwordService.CalculeHash(accountDeletion.Password);
                await LoginAs(user);
                LogData(user, accountDeletion);

                var response = await httpClient.PostAsJsonAsync(url, accountDeletion);
                response.StatusCode.Should().Be(HttpStatusCode.NoContent);

                await Execute(async db =>
                {
                    var dbUser = await db.Users.SingleAsync();
                    dbUser.Name.Should().StartWith(AccountConstants.AnonymizedPrefix);
                    dbUser.Username.Should().StartWith(AccountConstants.AnonymizedPrefix);
                    dbUser.IsActive.Should().BeFalse();

                    var sessions = await db.UserSessions.ToArrayAsync();
                    sessions.Should().BeEmpty();
                });
            }
        }

        public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task WithWrongPassword_ReturnsForbidden()
            {
                var passwordService = Require<IPasswordService>();
                var accountDeletion = new AccountDeletionDtoFaker().Generate();
                var wrongAccountDeletion = new AccountDeletionDtoFaker().Generate();
                var user = new UserFaker().Generate();
                user.PasswordHash = passwordService.CalculeHash(accountDeletion.Password);
                await LoginAs(user);
                LogData(user, accountDeletion);

                var response = await httpClient.PostAsJsonAsync(url, wrongAccountDeletion);
                response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

                await Execute(async db =>
                {
                    var dbUser = await db.Users.SingleAsync();
                    dbUser.Name.Should().Be(user.Name);
                    dbUser.Username.Should().Be(user.Username);
                    dbUser.IsActive.Should().BeTrue();

                    var sessions = await db.UserSessions.ToArrayAsync();
                    sessions.Should().ContainSingle();
                });
            }
        }
    }
}
