using Microsoft.Playwright;
using OverkillDocs.Tests.E2E.Collections;
using OverkillDocs.Tests.E2E.Fixtures;

namespace OverkillDocs.Tests.E2E.Tests;

[Collection(OkdCollection.Name)]
public abstract class TestBase(PlaywrightFixture fixture)
{
    protected IBrowser Browser { get => fixture.Browser; }
    public readonly RouteCollection Routes = new(fixture.BaseUrl);

    public record RouteCollection(string Root)
    {
        public readonly string Health = $"{Root}/health";
        public readonly string Login = $"{Root}/login";
    }
}
