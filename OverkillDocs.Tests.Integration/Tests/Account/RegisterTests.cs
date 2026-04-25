using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Tests.Integration.Fakers.DTOs.Account;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests.Account
{
    public class RegisterTests
    {
        private static readonly string url = "/api/account/register";

        public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task ValidData_CreatesUserAndSession()
            {
                var data = new AuthRequestDtoFaker().Generate();
                LogData(data);

                var response = await httpClient.PostAsJsonAsync(url, data);
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result?.Token.Should().NotBeNullOrEmpty();
                await Execute(async db =>
                {
                    var user = await db.Users.SingleAsync();
                    user.Username.Should().Be(data.Username);

                    var session = await db.UserSessions.SingleAsync();
                    session.UserId.Should().Be(user.Id);
                });
            }
        }

        public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task InvalidData_ReturnsBadRequest()
            {
                var data = new AuthRequestDtoFaker().Generate() with { Password = string.Empty };
                LogData(data);

                var response = await httpClient.PostAsJsonAsync(url, data);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                await Execute(async db =>
                {
                    var users = await db.Users.ToArrayAsync();
                    users.Should().BeEmpty();
                });
            }

            [Fact]
            public async Task UserExists_ReturnsConflict()
            {
                var user = new UserFaker().Generate();
                var data = new AuthRequestDtoFaker().Generate() with { Username = user.Username };
                LogData(user, data);
                await ExecuteAndCommit(db => db.Users.Add(user));

                var response = await httpClient.PostAsJsonAsync(url, data);

                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
                await Execute(async db =>
                {
                    var users = await db.Users.ToArrayAsync();
                    users.Should().ContainSingle();
                });
            }
        }
    }
}
