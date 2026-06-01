namespace OverkillDocs.Tests.E2E.Components;

internal sealed class SidebarComponent(IPage page)
{
    private ILocator Rail => page.GetByTestId("sidebar-rail");
    private ILocator Sidenav => page.Locator("mat-sidenav");

    private ILocator ChatButton => Rail.GetByTestId("chat-button");
    private ILocator DocumentsButton => Rail.GetByTestId("documents-button");
    private ILocator DebugButton => Rail.GetByTestId("debug-button");
    private ILocator[] RailButtons => [ChatButton, DocumentsButton, DebugButton];

    public FileIndexComponent FileIndex { get; } = new(page);
    public ChatComponent Chat { get; } = new(page);


    public async Task Close()
    {
        var openedClass = "mat-drawer-opened";
        var animatingClass = "mat-drawer-animating";
        await Expect(Sidenav).Not.ToHaveClassAsync(animatingClass);

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
        await Expect(Sidenav).Not.ToHaveClassAsync(animatingClass);
        await Expect(Sidenav).Not.ToHaveClassAsync(openedClass);
    }

    public async Task ShowFileIndex()
    {
        if (!await FileIndex.IsVisible())
            await DocumentsButton.ClickAsync();

        await FileIndex.ExpectToBeVisible();
    }

    public async Task ShowChat()
    {
        if (!await Chat.IsVisible())
            await ChatButton.ClickAsync();

        await Chat.ExpectToBeVisible();
    }

    private static async Task<bool> IsActiveButtonButton(ILocator button)
    {
        return (await button.GetAttributeAsync("class") ?? "").Split(" ").Contains("active");
    }
}
