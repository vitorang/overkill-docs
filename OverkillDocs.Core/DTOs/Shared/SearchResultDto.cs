namespace OverkillDocs.Core.DTOs.Shared;

public sealed record SearchResultDto<T>(
    string Text,
    int Page,
    int Total,
    bool HasMore,
    T[] Items
);
