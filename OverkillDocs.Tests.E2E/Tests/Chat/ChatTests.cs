using OverkillDocs.Tests.E2E.Components;

namespace OverkillDocs.Tests.E2E.Tests.Chat;

[Collection(OkdCollection.Name)]
public class ChatTests(PlaywrightFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task ChatMessagesShouldSync()
    {
        var (_, desktopPage) = await NewBrowserSession();
        var (_, mobilePage) = await NewBrowserSession(mobile: true);

        var message1 = Ulid.NewUlid().ToString();
        var message2 = Ulid.NewUlid().ToString();
        LogData(message1, message2);

        var desktopSidebar = new SidebarComponent(desktopPage);
        var mobileSidebar = new SidebarComponent(mobilePage);

        await desktopPage.GotoAsync(Routes.Documents);
        await desktopSidebar.ShowChat();
        await desktopSidebar.Chat.ExpectMessagingIsEnabled();
        await desktopSidebar.Chat.SendMessage(message1);
        await desktopSidebar.Chat.ExpectMessageReceived(message1);

        await mobilePage.GotoAsync(Routes.Documents);
        await mobileSidebar.ShowChat();
        await mobileSidebar.Chat.ExpectMessagingIsEnabled();
        await mobileSidebar.Chat.ExpectMessageReceived(message1);

        await mobileSidebar.Chat.SendMessage(message2);
        await mobileSidebar.Chat.ExpectMessageReceived(message2);
        await desktopSidebar.Chat.ExpectMessageReceived(message2);
    }
}
