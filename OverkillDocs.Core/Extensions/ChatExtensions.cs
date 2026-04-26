using HashidsNet;
using OverkillDocs.Core.DTOs.Chat;
using OverkillDocs.Core.Entities.Chat;

namespace OverkillDocs.Core.Extensions;

public static class ChatExtensions
{
    public static ChatMessageDto ToDto(this ChatMessage entity, IHashids hashids)
    {
        return new ChatMessageDto
        (
            Id: entity.Id,
            Content: entity.Content,
            UserHashId: hashids.Encode(entity.UserId),
            Timestamp: entity.Timestamp
        );
    }

    public static ChatMessage ToEntity(this ChatMessageDto dto, IHashids hashids)
    {
        return new ChatMessage
        (
            Id: dto.Id,
            Content: dto.Content,
            UserId: hashids.Decode(dto.UserHashId).First(),
            Timestamp: dto.Timestamp
        );
    }
}
