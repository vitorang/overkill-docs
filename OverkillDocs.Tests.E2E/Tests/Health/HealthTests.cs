namespace OverkillDocs.Tests.E2E.Tests.Health;


[Collection(OkdCollection.Name)]
public class HealthTests(PlaywrightFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task SystemIsUp_ReturnsHealthy()
    {
        var page = await Browser.NewPageAsync();

        var response = await page.GotoAsync(Routes.Health);

        response!.Status.Should().Be(200);

        await Expect(page.Locator("body")).ToContainTextAsync("Healthy");
    }
}
