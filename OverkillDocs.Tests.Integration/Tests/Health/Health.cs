using FluentAssertions;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net;

namespace OverkillDocs.Tests.Integration.Tests.Health
{
    public class Health
    {
        public class WhenSystemIsUp(TestFactory factory) : IClassFixture<TestFactory>
        {
            private readonly HttpClient httpClient = factory.CreateClient();

            [Fact]
            public async Task ShouldReturnOk()
            {
                var response = await httpClient.GetAsync("/health");
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
    }
}
