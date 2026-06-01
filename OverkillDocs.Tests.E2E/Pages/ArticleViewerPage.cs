using OverkillDocs.Tests.E2E.Components.Modals;

namespace OverkillDocs.Tests.E2E.Pages;

internal sealed class ArticleViewerPage(IPage page)
{
    private ILocator Component => page.Locator("okd-article-viewer");
    private ILocator Title => Component.GetByTestId("title");
    private ILocator ActionsButton => Component.GetByTestId("actions-button");

    private ILocator ActionsMenu => page.Locator(".mat-mdc-menu-content");
    private ILocator EditButton => ActionsMenu.GetByTestId("edit-button");
    private ILocator RenameButton => ActionsMenu.GetByTestId("rename-button");
    private ILocator DeleteButton => ActionsMenu.GetByTestId("delete-button");

    private ConfirmModalComponent ConfirmModal { get; } = new(page);
    private DocumentEditModalComponent EditModal { get; } = new(page);

    public async Task ExpectTitleToBe(string title)
    {
        await Expect(Title).ToBeVisibleAsync();
        await Expect(Title).ToContainTextAsync(title);
    }

    public async Task ChangeTitle(string title)
    {
        await Expect(ActionsButton).ToBeVisibleAsync();
        await ActionsButton.ClickAsync();
        await Expect(RenameButton).ToBeVisibleAsync();
        await RenameButton.ClickAsync();
        await EditModal.SetTitleAndSubmit(title);
    }

    public async Task DeleteCurrentDocument()
    {
        await Expect(ActionsButton).ToBeVisibleAsync();
        await ActionsButton.ClickAsync();
        await Expect(DeleteButton).ToBeVisibleAsync();
        await DeleteButton.ClickAsync();
        await ConfirmModal.Confirm();
    }
}
