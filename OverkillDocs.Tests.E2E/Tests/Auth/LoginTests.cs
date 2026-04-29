namespace OverkillDocs.Tests.E2E.Tests.Auth;

[Collection(OkdCollection.Name)]
public class LoginTests(PlaywrightFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task WhenUserLogin_RedirectsToDocuments()
    {
        var page = await Browser.NewPageAsync();
        var loginPage = new LoginPage(page);
        var (auth, _) = await CreateUserAuth();
        LogData(auth);

        await page.GotoAsync(Routes.Login);
        await loginPage.Login(auth);

        await Expect(page).ToHaveURLAsync(Routes.Documents);
    }
}
