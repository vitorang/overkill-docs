namespace OverkillDocs.Tests.Integration.Tests.DocumentsController;

public class DeleteTests
{
    private static string Url(string hashId) => $"/api/Documents/{hashId}";

    public class Success(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task DeletesDocumentAndFragments_ReturnsNoContent()
        {
            var summaryCache = Require<IObjectCache<DocumentSummariesResult>>();
            var fragmentIdsCache = Require<IObjectCache<DocumentFragmentHashIdsResult>>();
            await LoginAs(new UserFaker().Generate());

            var document = new DocumentFaker().Generate();
            var fragment = new DocumentFragmentFaker(document).Generate();
            await ExecuteAndCommit(db =>
            {
                db.Documents.Attach(document);
                db.DocumentFragments.Attach(fragment);
            });

            var summaries = new DocumentSummariesResult([document.ToSummaryDto(Hashids)]);
            var fragmentIds = new DocumentFragmentHashIdsResult(DocumentId: document.Id, FragmentHashIds: [Hashids.Encode(fragment.Id)]);
            await summaryCache.Set(summaries);
            await fragmentIdsCache.Set(fragmentIds);

            var hashId = Hashids.Encode(document.Id);
            LogData(document, hashId, fragment);

            var response = await httpClient.DeleteAsync(Url(hashId));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await Execute(async db =>
            {
                db.Documents.Should().BeEmpty();
                db.DocumentFragments.Should().BeEmpty();
            });

            var cachedSummary = await summaryCache.Get(summaryCache.IdFrom(summaries));
            cachedSummary.Should().BeNull();

            var cachedHashIds = await fragmentIdsCache.Get(fragmentIds.DocumentId);
            cachedHashIds.Should().BeNull();
        }
    }

    public class Failure(TestFactory factory, ITestOutputHelper outputHelper) : TestBase(factory, outputHelper)
    {
        [Fact]
        public async Task DocumentDoesNotExist_ReturnsNotFound()
        {
            await LoginAs(new UserFaker().Generate());
            var (document, fragment, fragmentIds, summaries) = await InsertData();

            var hashId = Hashids.Encode(document.Id + 1);
            LogData(document, hashId, fragment);

            var response = await httpClient.DeleteAsync(Url(hashId));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            await StoredDataShouldNotBeDeleted(fragmentIds, summaries);
        }

        [Fact]
        public async Task DocumentFragmentIsLocked_ReturnsConflict()
        {
            var lockCache = Require<IObjectCache<DocumentFragmentLockDto>>();

            var user = new UserFaker().Generate();
            await LoginAs(user);
            var (document, fragment, fragmentIds, summaries) = await InsertData();

            var fragmentLock = new DocumentFragmentLockDto(
                FragmentHashId: Hashids.Encode(fragment.Id),
                UserHashId: Hashids.Encode(user.Id));

            await lockCache.Set(fragmentLock);

            var hashId = Hashids.Encode(document.Id);
            LogData(document, hashId, fragment, fragmentLock);

            var response = await httpClient.DeleteAsync(Url(hashId));
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            await StoredDataShouldNotBeDeleted(fragmentIds, summaries);
        }

        private async Task<(Document document, DocumentFragment fragment, DocumentFragmentHashIdsResult fragmentIds, DocumentSummariesResult summaries)> InsertData()
        {
            var summaryCache = Require<IObjectCache<DocumentSummariesResult>>();
            var fragmentIdsCache = Require<IObjectCache<DocumentFragmentHashIdsResult>>();
            var document = new DocumentFaker().Generate();
            var fragment = new DocumentFragmentFaker(document).Generate();
            await ExecuteAndCommit(db =>
            {
                db.Documents.Attach(document);
                db.DocumentFragments.Attach(fragment);
            });

            var summaries = new DocumentSummariesResult([document.ToSummaryDto(Hashids)]);
            var fragmentIds = new DocumentFragmentHashIdsResult(DocumentId: document.Id, FragmentHashIds: [Hashids.Encode(fragment.Id)]);
            await summaryCache.Set(summaries);
            await fragmentIdsCache.Set(fragmentIds);

            return (document, fragment, fragmentIds, summaries);
        }

        private async Task StoredDataShouldNotBeDeleted(DocumentFragmentHashIdsResult fragmentIds, DocumentSummariesResult summaries)
        {
            var summaryCache = Require<IObjectCache<DocumentSummariesResult>>();
            var fragmentIdsCache = Require<IObjectCache<DocumentFragmentHashIdsResult>>();

            await Execute(async db =>
            {
                db.Documents.Should().NotBeEmpty();
                db.DocumentFragments.Should().NotBeEmpty();
            });

            var cachedSummary = await summaryCache.Get(summaryCache.IdFrom(summaries));
            cachedSummary.Should().NotBeNull();

            var cachedHashIds = await fragmentIdsCache.Get(fragmentIds.DocumentId);
            cachedHashIds.Should().NotBeNull();
        }
    }
}
