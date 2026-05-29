namespace OverkillDocs.Tests.Integration.Tests.DocumentsController;

public class CreateTests
{
    private static readonly string url = "/api/Documents";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task ReturnsCreatedDocument()
        {
            await LoginAs(new UserFaker().Generate());

            var document = new DocumentSummaryDtoFaker().Generate();
            LogData(document);

            var response = await httpClient.PostAsJsonAsync(url, document);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DocumentSummaryDto>();

            await Execute(async db =>
            {
                var dbDocument = await db.Documents.SingleAsync();
                dbDocument.Title.Should().Be(document.Title);
            });

            result.Should().NotBeNull();
            result.HashId.Should().NotBeNullOrEmpty();
            var expected = document with { HashId = result.HashId };
            result.Should().Be(expected);
        }
    }
}
