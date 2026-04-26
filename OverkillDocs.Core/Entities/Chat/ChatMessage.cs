namespace OverkillDocs.Core.Entities.Chat;

public sealed record ChatMessage(
    string Id,
    string Content,
    int UserId,
    DateTime Timestamp
);
