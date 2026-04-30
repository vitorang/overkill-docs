namespace OverkillDocs.Tests.E2E.Components;

internal sealed class ReconnectionOverlayComponent(IPage page)
{
    private ILocator Component => page.Locator("okd-reconnection-overlay");

    public async Task ExpectIsConnected()
    {
        await Expect(Component).ToBeVisibleAsync();
        await Expect(Component).ToHaveAttributeAsync("data-connected", "true");
    }

    public async Task ExpectIsDisconnected()
    {
        await Expect(Component).ToBeVisibleAsync();
        await Expect(Component).ToHaveAttributeAsync("data-connected", "false");
    }
}
