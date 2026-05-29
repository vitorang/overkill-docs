namespace OverkillDocs.Tests.Integration.Tests.DocumentsController;

public class ListTests
{
    private static readonly string url = "/api/Documents";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task ReturnsDocuments()
        {
            var summaryCache = Require<IObjectCache<DocumentSummariesResult>>();
            await LoginAs(new UserFaker().Generate());

            var document1 = new DocumentFaker().Generate();
            var document2 = new DocumentFaker().Generate();

            await ExecuteAndCommit(db =>
                db.Documents.AttachRange(document1, document2)
            );
            LogData(document1, document2);

            var response = await httpClient.GetAsync(url);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var documents = await response.Content.ReadFromJsonAsync<DocumentSummaryDto[]>();

            documents.Should().HaveCount(2);
            documents.Should().Contain(document1.ToSummaryDto(Hashids));
            documents.Should().Contain(document2.ToSummaryDto(Hashids));

            var cachedValue = await summaryCache.Get(summaryCache.IdFrom(new([])));
            cachedValue.Should().NotBeNull();
            cachedValue.Documents.Should().BeEquivalentTo(documents);
        }
    }
}
