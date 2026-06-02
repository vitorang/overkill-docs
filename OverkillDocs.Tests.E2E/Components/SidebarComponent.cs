namespace OverkillDocs.Tests.E2E.Components;

internal sealed class SidebarComponent(IPage page)
{
    private ILocator Rail => page.GetByTestId("sidebar-rail");
    private ILocator Sidenav => page.Locator("mat-sidenav");

    private ILocator ChatButton => Rail.GetByTestId("chat-button");
    private ILocator DocumentsButton => Rail.GetByTestId("documents-button");
    private ILocator DebugButton => Rail.GetByTestId("debug-button");
    private ILocator[] RailButtons => [ChatButton, DocumentsButton, DebugButton];

    public DocumentIndexComponent DocumentIndex { get; } = new(page);
    public ChatComponent Chat { get; } = new(page);


    public async Task Close()
    {
        var openedClass = "mat-drawer-opened";
        await ExpectToNotAnimating();

        var isOpened = (await Sidenav.GetAttributeAsync("class") ?? "").Split(" ").Contains(openedClass);
        if (!isOpened)
            return;

        ILocator? activeButton = null;
        foreach (var button in RailButtons)
        {
            if (await IsActiveButtonButton(button))
            {
                activeButton = button;
                break;
            }
        }

        Assert.NotNull(activeButton);
        await activeButton.ClickAsync();
        await ExpectToNotAnimating();
        await Expect(Sidenav).Not.ToHaveClassAsync(openedClass);
    }

    public async Task ShowDocumentIndex()
    {
        if (!await DocumentIndex.IsVisible())
        {
            await DocumentsButton.ClickAsync();
            await ExpectToNotAnimating();
        }

        await DocumentIndex.ExpectToBeVisible();
    }

    public async Task ShowChat()
    {
        if (!await Chat.IsVisible())
        {
            await ChatButton.ClickAsync();
            await ExpectToNotAnimating();
        }

        await Chat.ExpectToBeVisible();
    }

    private static async Task<bool> IsActiveButtonButton(ILocator button)
    {
        return (await button.GetAttributeAsync("class") ?? "").Split(" ").Contains("active");
    }

    private async Task ExpectToNotAnimating()
    {
        var animatingClass = "mat-drawer-animating";
        await Expect(Sidenav).Not.ToHaveClassAsync(animatingClass);
    }

}
