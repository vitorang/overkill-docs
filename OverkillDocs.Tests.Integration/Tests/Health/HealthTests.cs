using FluentAssertions;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;

namespace OverkillDocs.Tests.Integration.Tests.Health;

public class HealthTests(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient httpClient = factory.CreateClient();

    [Fact]
    public async Task SystemIsUp_ReturnsHealthy()
    {
        var response = await httpClient.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsStringAsync();
        result.Should().Be("Healthy");
    }
}
