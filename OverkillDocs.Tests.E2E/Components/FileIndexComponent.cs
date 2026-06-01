namespace OverkillDocs.Tests.E2E.Components;

internal sealed class FileIndexComponent(IPage page)
{
    private ILocator Component => page.Locator("okd-document-index");

    public async Task<bool> IsVisible()
    {
        return await Component.IsVisibleAsync();
    }

    public async Task ExpectToBeVisible()
    {
        await Expect(Component).ToBeVisibleAsync();
    }

    public async Task AddArticle(string title)
    {

    }
}
