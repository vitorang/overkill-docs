using HashidsNet;
using OverkillDocs.Core.DTOs.Chat;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Services;

internal sealed class ChatService(IChatRepository chatRepository, IHashids hashids) : IChatService
{
    public async Task AddRecent(ChatMessageDto messageDto)
    {
        var message = messageDto.ToEntity(hashids);
        await chatRepository.AddRecentMessage(message);
    }

    public async Task<ImmutableList<ChatMessageDto>> GetRecent()
    {
        var recentMessages = await chatRepository.GetHistory();
        return [.. recentMessages.Select(e => e.ToDto(hashids))];
    }
}
