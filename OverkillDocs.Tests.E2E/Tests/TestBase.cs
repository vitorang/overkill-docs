using System.Net.Http.Json;
using System.Text.Json;

namespace OverkillDocs.Tests.E2E.Tests;

[Collection(OkdCollection.Name)]
public abstract class TestBase(PlaywrightFixture fixture, ITestOutputHelper outputHelper)
{
    protected IBrowser Browser { get => fixture.Browser; }
    public readonly RouteCollection Routes = new(fixture.BaseUrl);
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
    private readonly HttpClient httpClient = new();


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

    public record RouteCollection(string Root)
    {
        public readonly string ApiRegister = $"{Root}/api/account/register";

        public readonly string Documents = $"{Root}/documents";
        public readonly string Health = $"{Root}/health";
        public readonly string Login = $"{Root}/account/login";
    }
}
