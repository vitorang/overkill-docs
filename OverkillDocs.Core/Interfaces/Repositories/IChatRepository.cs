using OverkillDocs.Core.Entities.Chat;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Interfaces.Repositories;

public interface IChatRepository
{
    Task AddRecentMessage(ChatMessage message);
    Task<ImmutableArray<ChatMessage>> GetHistory();
}
