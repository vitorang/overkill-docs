using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Tests.Integration.Fakers;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace OverkillDocs.Tests.Integration.Tests.Account
{
    public class Register
    {
        private static readonly string url = "/api/account/register";

        public class WhenDataIsInvalid(TestFactory factory) : TestBase(factory)
        {
            [Fact]
            public async Task ShouldReturnBadRequest()
            {
                var data = new AuthRequestDtoFaker().Generate() with { Password = string.Empty };

                var response = await httpClient.PostAsJsonAsync(url, data);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                await ExecuteInScope<AppDbContext>(async dbContext =>
                {
                    var users = await dbContext.Users.ToArrayAsync();
                    users.Should().BeEmpty();
                });
            }
        }

        public class WhenDataIsValid(TestFactory factory) : TestBase(factory)
        {
            [Fact]
            public async Task ShouldReturnAuthResponse()
            {
                var data = new AuthRequestDtoFaker().Generate();

                var response = await httpClient.PostAsJsonAsync(url, data);
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result?.Token.Should().NotBeNullOrEmpty();
                await ExecuteInScope<AppDbContext>(async dbContext =>
                {
                    var user = await dbContext.Users.SingleAsync();
                    user.Username.Should().Be(data.Username);

                    var session = await dbContext.UserSessions.SingleAsync();
                    session.UserId.Should().Be(user.Id);
                });
            }
        }

        public class WhenUserExists(TestFactory factory) : TestBase(factory)
        {
            [Fact]
            public async Task ShouldReturnConflict()
            {
                var user = new UserFaker().Generate();
                var data = new AuthRequestDtoFaker().Generate() with { Username = user.Username };
                await ExecuteInScope<AppDbContext>(async dbContext =>
                {
                    dbContext.Users.Add(user);
                    await dbContext.SaveChangesAsync();
                });

                var response = await httpClient.PostAsJsonAsync(url, data);

                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
                await ExecuteInScope<AppDbContext>(async dbContext =>
                {
                    var users = await dbContext.Users.ToArrayAsync();
                    users.Should().HaveCount(1);
                });
            }
        }
    }
}
