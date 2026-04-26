using FluentAssertions;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests.AccountController;

public class SessionTests
{
    private static readonly string url = "/api/account/sessions";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task WhenMultipleUsersLogin_ReturnsCurrentUserSessions()
        {
            var hashIds = Require<IHashids>();

            var otherUser = new UserFaker().Generate();
            var otherSessions = await LoginMultiple(otherUser, 1);
            LogData(otherUser, otherSessions);

            var user = new UserFaker().Generate();
            var sessions = await LoginMultiple(user, 2);
            var session = sessions.Last();
            LogData($"Current session: {session.Id}", user, sessions);

            var response = await httpClient.GetAsync(url);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var sessionDtos = await response.Content.ReadFromJsonAsync<ImmutableArray<UserSessionDto>>();

            await Execute(async db =>
            {
                var dbSessions = await db.UserSessions.Where(e => e.UserId == user.Id).ToArrayAsync();
                dbSessions.Should().HaveCount(sessions.Length);
            });

            var sessionHashes = sessions.Select(e => hashIds.Encode(e.Id)).ToArray();
            var sessionDtoHashes = sessionDtos.Select(e => e.HashId).ToArray();
            sessionHashes.Should().BeEquivalentTo(sessionDtoHashes);

            var sessionHashId = hashIds.Encode(session.Id);
            sessionDtos.Single(e => e.IsCurrent).HashId.Should().Be(sessionHashId);
        }

        private async Task<ImmutableArray<UserSession>> LoginMultiple(User user, int count)
        {
            var sessions = Enumerable.Empty<UserSession>().ToList();
            foreach (var _ in Enumerable.Range(0, count))
                sessions.Add(await LoginAs(user));

            return [.. sessions];
        }
    }
}
