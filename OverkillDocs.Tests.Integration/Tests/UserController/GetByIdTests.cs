using FluentAssertions;
using HashidsNet;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests.UserController
{
    public class GetByIdTests
    {
        private static string UrlWithId(string hashId) => $"/api/user/{hashId}";

        public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task ReturnsOtherUserData()
            {
                var hashIds = Require<IHashids>();

                var otherUser = new UserFaker().Generate();
                await ExecuteAndCommit(db => db.Users.Add(otherUser));
                var otherUserHashId = hashIds.Encode(otherUser.Id);
                var user = new UserFaker().Generate();
                await LoginAs(user);
                LogData(user, otherUser, otherUserHashId);

                var response = await httpClient.GetAsync(UrlWithId(otherUserHashId));
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var simpleUser = await response.Content.ReadFromJsonAsync<SimpleUserDto>();

                simpleUser.Should().NotBeNull();
                simpleUser.HashId.Should().Be(otherUserHashId);
                simpleUser.Name.Should().Be(otherUser.Name);
            }
        }

        public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task WithInactiveUserId_ReturnsNotFound()
            {
                var hashIds = Require<IHashids>();

                var otherUser = new UserFaker().Generate();
                otherUser.IsActive = false;
                var otherUserHashId = hashIds.Encode(otherUser.Id);
                await ExecuteAndCommit(db => db.Users.Add(otherUser));
                
                var user = new UserFaker().Generate();
                await LoginAs(user);
                LogData(user, otherUser, otherUserHashId);

                var response = await httpClient.GetAsync(UrlWithId(otherUserHashId));
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }

            [Fact]
            public async Task WithNonExistentId_ReturnsNotFound()
            {
                var hashIds = Require<IHashids>();

                var user = new UserFaker().Generate();
                await LoginAs(user);
                var otherUserHashId = hashIds.Encode(user.Id + 1);
                LogData(user, otherUserHashId);

                var response = await httpClient.GetAsync(UrlWithId(otherUserHashId));
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}
