namespace OverkillDocs.Core.DTOs.Document;

public sealed record DocumentFragmentLockDto(
    string FragmentHashId,
    string UserHashId
);
