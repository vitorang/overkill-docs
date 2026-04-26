namespace OverkillDocs.Core.DTOs.Chat;

public record ChatMessageDto(
    string Id,
    string Content,
    string UserHashId,
    DateTime Timestamp
);
