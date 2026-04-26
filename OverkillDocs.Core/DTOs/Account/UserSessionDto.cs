namespace OverkillDocs.Core.DTOs.Account;

public sealed record UserSessionDto(
    string HashId,
    string UserAgent,
    bool IsCurrent
);
