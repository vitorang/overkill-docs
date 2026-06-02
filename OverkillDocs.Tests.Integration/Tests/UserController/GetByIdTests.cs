namespace OverkillDocs.Tests.Integration.Tests.UserController;

public class GetByIdTests
{
    private static string UrlWithId(string hashId) => $"/api/user/{hashId}";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task ReturnsOtherUserData()
        {
            var otherUser = new UserFaker().Generate();
            await ExecuteAndCommit(db => db.Users.Add(otherUser));
            var otherUserHashId = Hashids.Encode(otherUser.Id);
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
            var otherUser = new UserFaker().Generate();
            otherUser.IsActive = false;
            var otherUserHashId = Hashids.Encode(otherUser.Id);
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
            var user = new UserFaker().Generate();
            await LoginAs(user);
            var otherUserHashId = Hashids.Encode(user.Id + 1);
            LogData(user, otherUserHashId);

            var response = await httpClient.GetAsync(UrlWithId(otherUserHashId));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
