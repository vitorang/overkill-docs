using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Infrastructure.Interfaces;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests.Account
{
    public class ProfileTests
    {
        private static readonly string url = "/api/account/profile";

        public class GetSuccess(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task ReturnsOwnProfile()
            {
                var user = new UserFaker().Generate();
                var session = await LoginAs(user);
                LogData(user, session);

                var response = await httpClient.GetAsync(url);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();

                profile?.Username.Should().Be(user.Username);
            }
        }

        public class UpdateSuccess(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task WithProfileChange_ReturnsUpdatedProfile()
            {
                var hashIds = Require<HashidsNet.IHashids>();
                var user = new UserFaker().Generate();
                var cache = Require<IObjectCache<User>>();
                var session = await LoginAs(user);
                var profile = user.ToProfileDto(hashIds) with
                {
                    Avatar = "avatar",
                    Name = string.Join("", user.Name.Reverse())
                };
                await cache.Set(user);
                LogData(user, session, profile);

                var response = await httpClient.PostAsJsonAsync(url, profile);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var updatedProfile = await response.Content.ReadFromJsonAsync<ProfileDto>();

                updatedProfile.Should().BeEquivalentTo(profile);
                var cachedUser = await cache.Get(cache.IdFrom(user));
                cachedUser.Should().BeNull();
                await Execute(async db => {
                    var updatedUser = await db.Users.SingleAsync();
                    updatedUser.Name.Should().Be(profile.Name);
                    updatedUser.Avatar.Should().Be(profile.Avatar);
                });
            }
        }

        public class UpdateFailure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task WithUsernameChange_ReturnsForbidden()
            {
                var hashIds = Require<HashidsNet.IHashids>();
                var cache = Require<IObjectCache<User>>();

                var user = new UserFaker().Generate();
                var session = await LoginAs(user);
                var profile = user.ToProfileDto(hashIds) with { Username = "newusername" };
                await cache.Set(user);
                LogData(user, session, profile);

                var response = await httpClient.PostAsJsonAsync(url, profile);
                response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

                var cachedUser = await cache.Get(cache.IdFrom(user));
                cachedUser?.Username.Should().Be(user.Username);
                await Execute(async db => (await db.Users.FirstAsync()).Username.Should().Be(user.Username));
            }
        }
    }
}
