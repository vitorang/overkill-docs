using HashidsNet;
using OverkillDocs.Core.Constants;
using OverkillDocs.Core.DTOs.Chat;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Core.Services
{
    public class ChatService(IChatRepository chatRepository, IHashids hashids) : IChatService
    {
        public async Task AddRecent(ChatMessageDto messageDto, CancellationToken ct)
        {
            var message = messageDto.ToEntity(hashids);
            await chatRepository.AddRecentMessageAsync(message, ct);
        }

        public async Task<List<ChatMessageDto>> GetRecent(CancellationToken ct)
        {
            var history = await chatRepository.GetHistoryAsync(ct);
            return [.. history.RecentMessages(CacheConstants.ChatExpiration).Select(e => e.ToDto(hashids))];
        }
    }
}
