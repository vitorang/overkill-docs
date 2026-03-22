using HashidsNet;
using OverkillDocs.Core.DTOs.Chat;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Services
{
    public class ChatService(IChatRepository chatRepository, IHashids hashids) : IChatService
    {
        public async Task AddRecent(ChatMessageDto messageDto, CancellationToken ct)
        {
            var message = messageDto.ToEntity(hashids);
            await chatRepository.AddRecentMessageAsync(message, ct);
        }

        public async Task<ImmutableList<ChatMessageDto>> GetRecent(CancellationToken ct)
        {
            var recentMessages = await chatRepository.GetHistoryAsync(ct);
            return [.. recentMessages.Select(e => e.ToDto(hashids))];
        }
    }
}
