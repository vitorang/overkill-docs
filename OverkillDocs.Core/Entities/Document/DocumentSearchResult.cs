namespace OverkillDocs.Core.Entities.Document;

public sealed record DocumentSearchResult(
    string Text,
    int Page,
    int Total,
    bool HasMore,
    Document[] Items
);
