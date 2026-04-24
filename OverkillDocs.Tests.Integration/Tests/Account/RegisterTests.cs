using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Tests.Integration.Fakers;
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
                await ExecuteInScope<AppDbContext>(async dbContext =>
                {
                    var user = await dbContext.Users.SingleAsync();
                    user.Username.Should().Be(data.Username);

                    var session = await dbContext.UserSessions.SingleAsync();
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
                await ExecuteInScope<AppDbContext>(async dbContext =>
                {
                    var users = await dbContext.Users.ToArrayAsync();
                    users.Should().BeEmpty();
                });
            }

            [Fact]
            public async Task UserExists_ReturnsConflict()
            {
                var user = new UserFaker().Generate();
                var data = new AuthRequestDtoFaker().Generate() with { Username = user.Username };
                LogData(user, data);
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
                    users.Should().ContainSingle();
                });
            }
        }
    }
}
