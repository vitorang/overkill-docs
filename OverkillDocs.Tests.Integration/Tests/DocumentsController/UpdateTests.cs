namespace OverkillDocs.Tests.Integration.Tests.DocumentsController;

public class UpdateTests
{
    private static readonly string url = "/api/Documents";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task UpdatesDocument_ReturnsNoContent()
        {
            var summaryCache = Require<IObjectCache<DocumentSummariesResult>>();
            await LoginAs(new UserFaker().Generate());

            var document = new DocumentFaker().Generate();
            await ExecuteAndCommit(db =>
                db.Documents.Attach(document)
            );

            var summaries = new DocumentSummariesResult([document.ToSummaryDto(Hashids)]);
            var documentDto = new DocumentSummaryDtoFaker().Generate() with { HashId = Hashids.Encode(document.Id) };
            LogData(document, documentDto);

            var response = await httpClient.PutAsJsonAsync(url, documentDto);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await Execute(async db =>
            {
                var document = await db.Documents.SingleAsync();
                document.Title.Should().Be(documentDto.Title);
            });
            var cachedResult = await summaryCache.Get(summaryCache.IdFrom(summaries));
            cachedResult.Should().BeNull();
        }
    }

    public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task DocumentDoesNotExist_ReturnsNotFound()
        {
            var summaryCache = Require<IObjectCache<DocumentSummariesResult>>();
            await LoginAs(new UserFaker().Generate());

            var document = new DocumentFaker().Generate();
            var originalTitle = document.Title;
            await ExecuteAndCommit(db =>
                db.Documents.Attach(document)
            );
            var summaries = new DocumentSummariesResult([document.ToSummaryDto(Hashids)]);
            await summaryCache.Set(summaries);

            var documentDto = new DocumentSummaryDtoFaker().Generate() with { HashId = Hashids.Encode(document.Id + 1) };
            LogData(document, documentDto);

            var response = await httpClient.PutAsJsonAsync(url, documentDto);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await Execute(async db =>
            {
                var document = await db.Documents.SingleAsync();
                document.Title.Should().Be(originalTitle);
            });
            var cachedResult = await summaryCache.Get(summaryCache.IdFrom(summaries));
            cachedResult.Should().NotBeNull();
            cachedResult.Documents.Should().ContainSingle();
            cachedResult.Documents.First().Title.Should().Be(originalTitle);
        }
    }
}
