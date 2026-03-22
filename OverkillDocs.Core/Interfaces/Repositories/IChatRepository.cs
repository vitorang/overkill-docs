using OverkillDocs.Core.Entities.Chat;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task AddRecentMessageAsync(ChatMessage message, CancellationToken ct);
        Task<ImmutableList<ChatMessage>> GetHistoryAsync(CancellationToken ct);
    }
}
