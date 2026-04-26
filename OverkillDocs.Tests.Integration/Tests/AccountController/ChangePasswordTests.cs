using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Tests.Integration.Fakers.DTOs.Account;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests.AccountController
{
    public class ChangePasswordTests
    {
        private static readonly string url = "/api/account/change-password";

        public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task WithValidPassword_ChangesPasswordHash()
            {
                var passwordService = Require<IPasswordService>();

                var passwordChange = new PasswordChangeDtoFaker().Generate();
                var user = new UserFaker().Generate();
                user.PasswordHash = passwordService.CalculeHash(passwordChange.CurrentPassword);
                await LoginAs(user);
                LogData(user, passwordChange);

                var response = await httpClient.PostAsJsonAsync(url, passwordChange);
                response.StatusCode.Should().Be(HttpStatusCode.NoContent);

                await Execute(async db =>
                {
                    var dbUser = await db.Users.SingleAsync();
                    var passwordChanged = passwordService.VerifyPassword(passwordChange.NewPassword, dbUser.PasswordHash);
                    passwordChanged.Should().BeTrue();
                });
            }
        }


        public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task WithInvalidNewPassword_ReturnsBadRequest()
            {
                var passwordService = Require<IPasswordService>();

                var passwordChange = new PasswordChangeDtoFaker().Generate() with { NewPassword = "-" };
                var user = new UserFaker().Generate();
                user.PasswordHash = passwordService.CalculeHash(passwordChange.CurrentPassword);
                await LoginAs(user);
                LogData(user, passwordChange);

                var response = await httpClient.PostAsJsonAsync(url, passwordChange);
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                await Execute(async db =>
                {
                    var dbUser = await db.Users.SingleAsync();
                    var passwordUnchanged = passwordService.VerifyPassword(passwordChange.CurrentPassword, dbUser.PasswordHash);
                    passwordUnchanged.Should().BeTrue();
                });
            }

            [Fact]
            public async Task WithWrongPassword_ReturnsForbidden()
            {
                var passwordService = Require<IPasswordService>();

                var passwordChange = new PasswordChangeDtoFaker().Generate();
                var user = new UserFaker().Generate();
                user.PasswordHash = passwordService.CalculeHash(passwordChange.CurrentPassword);
                var wrongPassword = passwordChange with { CurrentPassword = passwordChange.CurrentPassword + "-" };
                await LoginAs(user);
                LogData(user, passwordChange, wrongPassword);

                var response = await httpClient.PostAsJsonAsync(url, wrongPassword);
                response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

                await Execute(async db =>
                {
                    var dbUser = await db.Users.SingleAsync();
                    var passwordUnchanged = passwordService.VerifyPassword(passwordChange.CurrentPassword, dbUser.PasswordHash);
                    passwordUnchanged.Should().BeTrue();
                });
            }
        }
    }
}
