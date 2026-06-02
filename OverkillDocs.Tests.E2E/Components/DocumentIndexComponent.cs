using OverkillDocs.Tests.E2E.Components.Modals;

namespace OverkillDocs.Tests.E2E.Components;

internal sealed class DocumentIndexComponent(IPage page)
{
    private ILocator Component => page.Locator("okd-document-index");
    private ILocator AddDocumentButton => page.GetByTestId("add-document-button");
    private DocumentEditModalComponent EditModal { get; } = new(page);

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
        await Expect(Component).ToBeVisibleAsync();
        await AddDocumentButton.ClickAsync();
        await EditModal.SetTitleAndSubmit(title);
    }

    public async Task ExpectDocumentVisibility(string title, bool visible)
    {
        var documentItem = Component.GetByText(title);
        if (visible)
            await Expect(documentItem).ToBeVisibleAsync();
        else
            await Expect(documentItem).Not.ToBeVisibleAsync();
    }

    public async Task SelectDocument(string title)
    {
        var documentItem = Component.GetByText(title);
        await Expect(documentItem).ToBeVisibleAsync();
        await documentItem.ClickAsync();
    }
}
