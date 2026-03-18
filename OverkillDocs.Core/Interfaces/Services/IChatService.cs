using OverkillDocs.Core.DTOs.Chat;

namespace OverkillDocs.Core.Interfaces.Services
{
    public interface IChatService
    {
        Task AddRecent(ChatMessageDto messageDto, CancellationToken ct);
        Task<List<ChatMessageDto>> GetRecent(CancellationToken ct);
    }
}
