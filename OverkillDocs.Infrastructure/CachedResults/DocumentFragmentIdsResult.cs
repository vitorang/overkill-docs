namespace OverkillDocs.Infrastructure.CachedResults;

internal sealed record DocumentFragmentIdsResult(
    int DocumentId,
    int[] FragmentIds
);
