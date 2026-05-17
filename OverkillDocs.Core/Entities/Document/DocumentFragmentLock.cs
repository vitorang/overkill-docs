namespace OverkillDocs.Core.Entities.Document;

public sealed record DocumentFragmentLock(
    int FragmentId,
    int UserId
);
