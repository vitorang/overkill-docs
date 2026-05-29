namespace OverkillDocs.Tests.Integration.Tests.UserController;

public class CurrentTests
{
    private static readonly string url = "/api/user/me";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task ReturnsOwnUserData()
        {
            var user = new UserFaker().Generate();
            await LoginAs(user);
            var userHashId = Hashids.Encode(user.Id);
            LogData(user);

            var response = await httpClient.GetAsync(url);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var simpleUser = await response.Content.ReadFromJsonAsync<SimpleUserDto>();

            simpleUser.Should().NotBeNull();
            simpleUser.HashId.Should().Be(userHashId);
            simpleUser.Name.Should().Be(user.Name);
        }
    }
}
