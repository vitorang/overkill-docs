namespace OverkillDocs.Core.DTOs.Account;

public record UserSessionDto(
    string HashId,
    string UserAgent,
    bool IsCurrent
);
