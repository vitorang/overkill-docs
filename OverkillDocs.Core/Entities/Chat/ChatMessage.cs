namespace OverkillDocs.Core.Entities.Chat;

public record ChatMessage(
    string Id,
    string Content,
    int UserId,
    DateTime Timestamp
);
