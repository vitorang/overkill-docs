namespace OverkillDocs.Tests.Integration.Tests.DocumentsController;

public class GetTests
{
    private static string Url(string hashId) => $"/api/Documents/{hashId}";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task ReturnsDocumentWithFragments()
        {
            await LoginAs(new UserFaker().Generate());

            var document = new DocumentFaker().Generate();
            var fragment = new DocumentFragmentFaker(document).Generate();
            await ExecuteAndCommit(db =>
            {
                db.Documents.Attach(document);
                db.DocumentFragments.Attach(fragment);
            });
            var hashId = Hashids.Encode(document.Id);
            LogData(document, hashId, fragment);

            var response = await httpClient.GetAsync(Url(hashId));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var documentDto = await response.Content.ReadFromJsonAsync<DocumentDetailDto>();
            documentDto.Should().NotBeNull();
            documentDto.Fragments.Should().NotBeEmpty();
            documentDto.Title.Should().Be(document.Title);

            var fragmentDto = documentDto.Fragments.First() as ArticleMarkdownFragmentDto;
            fragmentDto.Should().NotBeNull();
            fragment.Content.Should().Contain(fragmentDto.Text);
        }
    }

    public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task DocumentDoesNotExist_ReturnsNotFound()
        {
            await LoginAs(new UserFaker().Generate());

            var document = new DocumentFaker().Generate();
            var fragment = new DocumentFragmentFaker(document).Generate();
            await ExecuteAndCommit(db =>
            {
                db.Documents.Attach(document);
                db.DocumentFragments.Attach(fragment);
            });
            var hashId = Hashids.Encode(document.Id + 1);
            LogData(document, hashId, fragment);

            var response = await httpClient.GetAsync(Url(hashId));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
