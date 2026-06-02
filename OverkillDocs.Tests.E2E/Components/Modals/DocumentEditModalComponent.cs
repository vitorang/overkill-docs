namespace OverkillDocs.Tests.E2E.Components.Modals;

internal sealed class DocumentEditModalComponent(IPage page)
{
    private ILocator Form => page.GetByTestId("document-edit-form");
    private ILocator NameInput => Form.GetByTestId("name-input");
    private ILocator SubmitButton => Form.GetByTestId("submit-button");

    public async Task SetTitleAndSubmit(string title)
    {
        await Expect(Form).ToBeVisibleAsync();
        await NameInput.FillAsync(title);
        await SubmitButton.ClickAsync();
        await Expect(Form).Not.ToBeVisibleAsync();
    }
}
