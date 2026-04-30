namespace OverkillDocs.Tests.E2E.Components;

internal sealed class ChatComponent(IPage page)
{
    private ILocator Component => page.Locator("okd-chat-view");
    private ILocator MessageInput => Component.GetByTestId("message-input");
    private ILocator SendButton => Component.GetByTestId("send-button");
    private ILocator ChatMessage => Component.Locator("okd-chat-message");

    public async Task ExpectMessagingIsEnabled()
    {
        await Expect(MessageInput).ToBeEnabledAsync();
        await Expect(SendButton).ToBeEnabledAsync();
    }

    public async Task ExpectMessagingIsDisabled()
    {
        await Expect(MessageInput).ToBeDisabledAsync();
        await Expect(SendButton).ToBeDisabledAsync();
    }

    public async Task SendMessage(string message)
    {
        await MessageInput.FillAsync(message);
        await SendButton.ClickAsync();
    }

    public async Task SendMessageWithEnter(string message)
    {
        await MessageInput.FillAsync(message);
        await page.Keyboard.PressAsync("Enter");
    }

    public async Task ExpectMessageReceived(string messageText)
    {
        var chatMessage = ChatMessage.Filter(new()
        {
            Has = page.GetByTestId("message-text"),
            HasText = messageText
        });

        await Expect(chatMessage).ToBeVisibleAsync();
    }

    public async Task ExpectToBeVisible()
    {
        await Expect(Component).ToBeVisibleAsync();
    }
}
