namespace OverkillDocs.Core.DTOs.Chat
{
    public record ChatMessageDto(
        string Content,
        string UserHashId,
        DateTime Timestamp
    );
}
