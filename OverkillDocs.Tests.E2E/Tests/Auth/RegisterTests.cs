namespace OverkillDocs.Tests.E2E.Tests.Auth;

[Collection(OkdCollection.Name)]
public class RegisterTests(PlaywrightFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task WhenUserRegister_RedirectsToDocuments()
    {
        var (_, page) = await NewBrowserSession(authUser: false);
        var loginPage = new LoginPage(page);
        var auth = new AuthRequestDtoFaker().Generate();
        LogData(auth);

        await page.GotoAsync(Routes.Login);
        await loginPage.Register(auth);

        await Expect(page).ToHaveURLAsync(Routes.Documents);
    }
}
