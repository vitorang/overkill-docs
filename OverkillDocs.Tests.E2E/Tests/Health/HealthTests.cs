using FluentAssertions;
using Microsoft.Playwright;
using OverkillDocs.Tests.E2E.Collections;
using OverkillDocs.Tests.E2E.Fixtures;

namespace OverkillDocs.Tests.E2E.Tests.Health;


[Collection(OkdCollection.Name)]
public class HealthTests(PlaywrightFixture fixture) : TestBase(fixture)
{
    [Fact]
    public async Task SystemIsUp_ReturnsHealthy()
    {
        var page = await Browser.NewPageAsync();

        var response = await page.GotoAsync(Routes.Health);

        response!.Status.Should().Be(200);

        await Assertions.Expect(page.Locator("body")).ToContainTextAsync("Healthy");
    }
}
