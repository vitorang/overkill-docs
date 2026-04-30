using OverkillDocs.Tests.E2E.Components;

namespace OverkillDocs.Tests.E2E.Pages;

internal sealed class DocumentsPage(IPage page)
{
    public ChatComponent Chat { get; } = new(page);
    public ReconnectionOverlayComponent ReconnectionOverlay = new(page);
    private ILocator Toolbar => page.Locator(TestConstants.ToolbarTag);
    private ILocator ToggleChatButton => Toolbar.GetByTestId("toggle-chat-button");

    public async Task EnsureChatIsVisible()
    {
        await Expect(Toolbar).ToBeVisibleAsync();

        if (await ToggleChatButton.IsVisibleAsync())
        {
            var currentState = await ToggleChatButton.GetAttributeAsync("data-value");
            if (currentState != "chat")
                await ToggleChatButton.ClickAsync();
        }

        await Chat.ExpectToBeVisible();
    }
}
