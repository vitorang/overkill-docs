using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Tests.Integration.Fakers.DTOs.Account;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests.Account
{
    public class LoginTests
    {
        private static readonly string url = "/api/account/login";

        public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            private readonly User user = new UserFaker().Generate();
            private readonly string password = "password";

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();

                var passwordService = Require<IPasswordService>();
                user.PasswordHash = passwordService.CalculeHash(password);
                await ExecuteAndCommit(db => db.Users.Add(user));
            }

            [Fact]
            public async Task ValidData_AuthenticatesUser()
            {
                var data = new AuthRequestDtoFaker().Generate()
                    with
                { Username = user.Username, Password = password };
                LogData(user, data);

                var response = await httpClient.PostAsJsonAsync(url, data);
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result?.Token.Should().NotBeNullOrEmpty();

                await Execute(async db =>
                {
                    var session = await db.UserSessions.SingleAsync();
                    session.UserId.Should().Be(user.Id);
                    session.Token.Should().Be(result!.Token);
                });
            }
        }

        public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            private readonly User user = new UserFaker().Generate();
            private readonly string password = "password";

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();

                var passwordService = Require<IPasswordService>();
                user.PasswordHash = passwordService.CalculeHash(password);
                await ExecuteAndCommit(db => db.Users.Add(user));
            }

            [Fact]
            public async Task InvalidData_ReturnsBadRequest()
            {
                var data = new AuthRequestDtoFaker().Generate() with { Password = string.Empty };
                LogData(user, data);

                var response = await httpClient.PostAsJsonAsync(url, data);

                await AssertNoSessionCreated(response, HttpStatusCode.BadRequest);
            }

            [Fact]
            public async Task WrongUsername_ReturnsNotFound()
            {
                var data = new AuthRequestDtoFaker().Generate() with { Username = "test" };
                LogData(data);

                var response = await httpClient.PostAsJsonAsync(url, data);

                await AssertNoSessionCreated(response, HttpStatusCode.NotFound);
            }

            [Fact]
            public async Task WrongPassword_ReturnsNotFound()
            {
                var data = new AuthRequestDtoFaker().Generate() with { Password = $"{password}-test" };
                LogData(data);

                var response = await httpClient.PostAsJsonAsync(url, data);

                await AssertNoSessionCreated(response, HttpStatusCode.NotFound);
            }

            private async Task AssertNoSessionCreated(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
            {
                response.StatusCode.Should().Be(expectedStatusCode);

                await Execute(async db =>
                {
                    var sessions = await db.UserSessions.ToArrayAsync();
                    sessions.Should().BeEmpty();
                });
            }
        }
    }
}
