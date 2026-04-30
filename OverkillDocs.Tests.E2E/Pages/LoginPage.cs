namespace OverkillDocs.Tests.E2E.Pages;

internal sealed class LoginPage(IPage page)
{
    private ILocator Component => page.Locator("okd-auth-form");

    private ILocator UsernameInput => Component.GetByTestId("username-input");
    private ILocator PasswordInput => Component.GetByTestId("password-input");
    private ILocator SubmitButton => Component.GetByTestId("submit-button");
    private ILocator ToggleRegisterButton => Component.GetByTestId("toggle-register-button");

    public async Task Login(AuthRequestDto dto)
    {
        await Component.WaitForAsync();

        await UsernameInput.FillAsync(dto.Username);
        await PasswordInput.FillAsync(dto.Password);
        await SubmitButton.ClickAsync();
    }

    public async Task Register(AuthRequestDto dto)
    {
        await Component.WaitForAsync();

        await ToggleRegisterButton.ClickAsync();

        await UsernameInput.FillAsync(dto.Username);
        await PasswordInput.FillAsync(dto.Password);
        await SubmitButton.ClickAsync();
    }
}
