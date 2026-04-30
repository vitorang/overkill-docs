using System.Net.Http.Json;
using System.Text.Json;

namespace OverkillDocs.Tests.E2E.Tests;

[Collection(OkdCollection.Name)]
public abstract class TestBase(PlaywrightFixture fixture, ITestOutputHelper outputHelper) : IAsyncLifetime
{
    public record RouteCollection(string Root)
    {
        public readonly string ApiRegister = $"{Root}/api/account/register";

        public readonly string Documents = $"{Root}/documents";
        public readonly string Health = $"{Root}/health";
        public readonly string Login = $"{Root}/account/login";
    }

    public readonly RouteCollection Routes = new(fixture.BaseUrl);
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
    private readonly HttpClient httpClient = new();
    private readonly List<IBrowserContext> browserContexts = [];

    protected void LogData(params object?[] items)
    {
        int index = 0;
        foreach (var item in items)
        {
            var typeName = item?.GetType().Name ?? "NULL";
            var json = JsonSerializer.Serialize(item, jsonOptions);

            outputHelper.WriteLine($"#{index}: {typeName}\n{json}\n");
            index++;
        }
    }

    protected async Task<(AuthRequestDto auth, string authToken)> CreateUserAuth()
    {
        var auth = new AuthRequestDtoFaker().Generate();
        var response = await httpClient.PostAsJsonAsync(Routes.ApiRegister, auth);
        response.EnsureSuccessStatusCode();

        var authToken = (await response.Content.ReadFromJsonAsync<AuthResponseDto>())?.Token;
        if (string.IsNullOrEmpty(authToken))
            throw new InvalidOperationException($"Falha ao gerar usuário '{auth.Username}' com senha '{auth.Password}'");

        return (auth, authToken);
    }

    public static async Task WaitForConnectAsync(IPage page)
    {
        var overlay = page.Locator("okd-reconnection-overlay");
        await Expect(overlay).ToBeAttachedAsync();
        await Expect(overlay).ToHaveAttributeAsync("data-connected", "true");
    }

    public async Task<(IBrowserContext Context, IPage Page)> NewBrowserSession(bool mobile = false, bool authUser = true)
    {
        var options = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        };

        if (mobile)
        {
            options.ViewportSize.Width = 540;
            options.IsMobile = true;
            options.HasTouch = true;
        }

        var context = await fixture.Browser.NewContextAsync(options);
        browserContexts.Add(context);

        if (authUser)
        {
            var (auth, authToken) = await CreateUserAuth();
            LogData(nameof(NewBrowserSession), auth, authToken);
            await context.AddInitScriptAsync($"window.localStorage.setItem('{TestConstants.AuthToken}', '{authToken}');");
        }

        var page = await context.NewPageAsync();
        return (Context: context, Page: page);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var tasks = browserContexts.Select(c => c.CloseAsync());
        await Task.WhenAll(tasks);
    }
}
