namespace OverkillDocs.Infrastructure.CachedResults;

internal sealed record DocumentFragmentHashIdsResult(
    int DocumentId,
    string[] FragmentHashIds
);
