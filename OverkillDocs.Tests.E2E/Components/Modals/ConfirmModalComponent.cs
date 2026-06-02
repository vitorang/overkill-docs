namespace OverkillDocs.Tests.E2E.Components.Modals;

internal class ConfirmModalComponent(IPage page)
{
    private ILocator Form => page.GetByTestId("confirm-modal-actions");
    private ILocator ConfirmButton => Form.GetByTestId("confirm-button");

    public async Task Confirm()
    {
        await Expect(Form).ToBeVisibleAsync();
        await ConfirmButton.ClickAsync();
        await Expect(Form).Not.ToBeVisibleAsync();
    }
}
