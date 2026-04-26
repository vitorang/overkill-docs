namespace OverkillDocs.Core.DTOs.Chat;

public sealed record ChatMessageDto(
    string Id,
    string Content,
    string UserHashId,
    DateTime Timestamp
);
