namespace OverkillDocs.Tests.E2E.Tests.Chat;

[Collection(OkdCollection.Name)]
public class ChatTests(PlaywrightFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task ChatMessagesShouldBeSync()
    {
        var (_, desktopPage) = await NewBrowserSession();
        var (_, mobilePage) = await NewBrowserSession(mobile: true);

        var message1 = Ulid.NewUlid().ToString();
        var message2 = Ulid.NewUlid().ToString();
        LogData(message1, message2);

        var desktopDoc = new DocumentsPage(desktopPage);
        var mobileDoc = new DocumentsPage(mobilePage);

        await desktopPage.GotoAsync(Routes.Documents);
        await desktopDoc.EnsureChatIsVisible();
        await desktopDoc.Chat.ExpectMessagingIsEnabled();
        await desktopDoc.Chat.SendMessage(message1);
        await desktopDoc.Chat.ExpectMessageReceived(message1);

        await mobilePage.GotoAsync(Routes.Documents);
        await mobileDoc.EnsureChatIsVisible();
        await mobileDoc.Chat.ExpectMessagingIsEnabled();
        await mobileDoc.Chat.ExpectMessageReceived(message1);

        await mobileDoc.Chat.SendMessage(message2);
        await mobileDoc.Chat.ExpectMessageReceived(message2);
        await desktopDoc.Chat.ExpectMessageReceived("momomo");
    }

    [Fact]
    public async Task WhenHubConnectionLost_DisablesChat()
    {
        var (_, page) = await NewBrowserSession();
        var documentsPage = new DocumentsPage(page);

        await page.RouteAsync(TestConstants.HubRoute, route => route.AbortAsync());
        await page.GotoAsync(Routes.Documents);

        await documentsPage.EnsureChatIsVisible();
        await documentsPage.Chat.ExpectMessagingIsDisabled();

        await documentsPage.ReconnectionOverlay.ExpectIsDisconnected();
        await documentsPage.Chat.ExpectMessagingIsDisabled();
    }
}
