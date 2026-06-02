namespace OverkillDocs.Tests.E2E.Pages;

internal sealed class DocumentIntroPage(IPage page)
{
    private ILocator Component => page.Locator("okd-document-intro-page");

    public async Task ExpectToBeVisible()
    {
        await Expect(Component).ToBeVisibleAsync();
    }
}
