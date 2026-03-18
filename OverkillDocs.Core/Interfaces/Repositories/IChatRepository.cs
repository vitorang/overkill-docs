using OverkillDocs.Core.Entities.Chat;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task AddRecentMessageAsync(ChatMessage message, CancellationToken ct);
        Task<ChatHistory> GetHistoryAsync(CancellationToken ct);
    }
}
