using FluentAssertions;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests.Account
{
    public class LogoutTests
    {
        private static readonly string url = "/api/account/logout";

        private static string SessionUrl(string sessionHashId)
        {
            return $"{url}/{sessionHashId}";
        }

        public class CurrentSessionSuccess(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task AnonymousUser_DoesNotRemoveSession()
            {
                var user = new UserFaker().Generate();
                var session = new UserSessionFaker(user).Generate();
                await ExecuteAndCommit(db =>
                {
                    db.Users.Add(user);
                    db.UserSessions.Add(session);
                });
                LogData(user, session);

                var response = await httpClient.PostAsJsonAsync(url, new { });

                response.StatusCode.Should().Be(HttpStatusCode.NoContent);
                await Execute(async db => await db.UserSessions.SingleAsync());
            }

            [Fact]
            public async Task AuthenticatedUser_RemovesSession()
            {
                var user = new UserFaker().Generate();
                var session = await LoginAs(user);
                LogData(user, session);

                var response = await httpClient.PostAsJsonAsync(url, new { });

                response.StatusCode.Should().Be(HttpStatusCode.NoContent);
                await Execute(async db =>
                {
                    var sessions = await db.UserSessions.ToArrayAsync();
                    sessions.Should().BeEmpty();
                });
            }
        }

        public class OtherSessionSuccess(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            [Fact]
            public async Task AuthenticatedUser_RemovesSession()
            {
                var user = new UserFaker().Generate();
                var oldSession = await LoginAs(user);
                var newSession = await LoginAs(user);
                var hashIds = Require<IHashids>();
                LogData(user, oldSession, newSession);

                var response = await httpClient.PostAsJsonAsync(
                    SessionUrl(hashIds.Encode(oldSession.Id)), new { });

                await Execute(async db =>
                {
                    var sessions = await db.UserSessions.ToArrayAsync();
                    sessions.Should().ContainSingle();
                    sessions.Where(e => e.Token == newSession.Token).Should().ContainSingle();
                });
            }
        }

        public class OtherSessionFailure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
        {
            private int otherSessionId = 0;
            private string otherSessionHashId = string.Empty;

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();

                var user = new UserFaker().Generate();
                var session = new UserSessionFaker(user).Generate();

                await ExecuteAndCommit(db =>
                {
                    db.Users.Add(user);
                    db.UserSessions.Add(session);
                });

                otherSessionId = session.Id;
                otherSessionHashId = Require<IHashids>().Encode(otherSessionId);
                LogData(user, session, otherSessionHashId);
            }

            [Fact]
            public async Task AnonymousUser_ResultsUnauthorized()
            {
                var response = await httpClient.PostAsJsonAsync(SessionUrl(otherSessionHashId), new { });

                await AssertNoOtherSessionRemoved(response, HttpStatusCode.Unauthorized);
            }

            [Fact]
            public async Task LogoutOtherUserSession_ResultsForbidden()
            {
                var user = new UserFaker().Generate();
                var session = await LoginAs(user);
                LogData(user, session);

                var response = await httpClient.PostAsJsonAsync(SessionUrl(otherSessionHashId), new { });

                await AssertNoOtherSessionRemoved(response, HttpStatusCode.Forbidden);
            }

            [Fact]
            public async Task IdNoExists_ResultsNotFound()
            {
                var user = new UserFaker().Generate();
                var session = await LoginAs(user);
                LogData(user, session);

                var hashId = Require<IHashids>().Encode(0);
                var response = await httpClient.PostAsJsonAsync(SessionUrl(hashId), new { });

                await AssertNoOtherSessionRemoved(response, HttpStatusCode.NotFound);
            }

            private async Task AssertNoOtherSessionRemoved(HttpResponseMessage response, HttpStatusCode expectedCode)
            {
                response.StatusCode.Should().Be(expectedCode);
                await Execute(async db => await db.UserSessions.SingleAsync(e => e.Id == otherSessionId));
            }
        }
    }
}
