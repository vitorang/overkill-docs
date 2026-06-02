using OverkillDocs.Tests.E2E.Components.Modals;

namespace OverkillDocs.Tests.E2E.Pages;

internal sealed class ArticleViewerPage(IPage page)
{
    private ILocator Component => page.Locator("okd-article-viewer");
    private ILocator Title => Component.GetByTestId("title");
    private ILocator MenuContent => page.Locator(".mat-mdc-menu-content");
    private ILocator DisableEditButton => Component.GetByTestId("disable-edit-button");
    private ILocator FinishEditButton => page.GetByTestId("finish-edit-button");

    private ILocator ActionsButton => Component.GetByTestId("actions-button");
    private ILocator EditButton => MenuContent.GetByTestId("edit-button");
    private ILocator RenameButton => MenuContent.GetByTestId("rename-button");
    private ILocator DeleteButton => MenuContent.GetByTestId("delete-button");

    private ILocator AddImageButton => MenuContent.GetByTestId("add-image-button");
    private ILocator AddFragmentButton => Component
        .Locator("okd-article-add-fragment")
        .GetByTestId("add-fragment-button");
    private ILocator FragmentActionsButton => page
        .Locator("okd-article-edit-fragment")
        .GetByTestId("fragment-actions-button");
    private ILocator FragmentAvatar => page
        .Locator("okd-article-edit-fragment")
        .GetByTestId("fragment-avatar");
    private ILocator FragmentEditButton => MenuContent.GetByTestId("fragment-edit-button");
    private ILocator FragmentDeleteButton => MenuContent.GetByTestId("fragment-delete-button");

    private ILocator AltInput => page.GetByTestId("alt-input");
    private ILocator UrlInput => page.GetByTestId("url-input");

    private ConfirmModalComponent ConfirmModal { get; } = new(page);
    private DocumentEditModalComponent EditModal { get; } = new(page);

    public async Task ExpectTitleToBe(string title)
    {
        await Expect(Title).ToBeVisibleAsync();
        await Expect(Title).ToContainTextAsync(title);
    }

    public async Task ChangeTitle(string title)
    {
        await ActionsButton.ClickAsync();
        await RenameButton.ClickAsync();
        await EditModal.SetTitleAndSubmit(title);
    }

    public async Task DeleteCurrentDocument()
    {
        await ActionsButton.ClickAsync();
        await DeleteButton.ClickAsync();
        await ConfirmModal.Confirm();
    }

    public async Task EnableEditMode()
    {
        if (await DisableEditButton.IsVisibleAsync())
            return;

        await ActionsButton.ClickAsync();
        await EditButton.ClickAsync();
        await Expect(DisableEditButton).ToBeVisibleAsync();
    }

    public async Task CreateImageFragment(string url, string alt)
    {
        await AddFragmentButton.ClickAsync();
        await AddImageButton.ClickAsync();
        await FragmentActionsButton.ClickAsync();
        await FragmentEditButton.ClickAsync();
        await UrlInput.FillAsync(url);
        await AltInput.FillAsync(alt);
    }

    public async Task DeleteFragment()
    {
        await FragmentActionsButton.ClickAsync();
        await FragmentDeleteButton.ClickAsync();
        await ConfirmModal.Confirm();
    }

    public async Task ExpectFragmentLock(bool isLocked)
    {
        if (isLocked)
            await Expect(FragmentAvatar).ToBeVisibleAsync();
        else
            await Expect(FragmentAvatar).Not.ToBeVisibleAsync();
    }

    public async Task FinishEdit()
    {
        await FinishEditButton.ClickAsync();
    }

    public async Task ExpectElementWithAlt(string alt)
    {
        await Expect(page.GetByAltText(alt)).ToBeVisibleAsync();
    }

    public async Task ExpectNoElementWithAlt(string alt)
    {
        await Expect(page.GetByAltText(alt)).Not.ToBeVisibleAsync();
    }
}
