using OverkillDocs.Core.Entities.Chat;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class ChatRepository(IListCache<ChatMessage> chatMessageCache) : IChatRepository
    {
        public async Task AddRecentMessageAsync(ChatMessage message, CancellationToken ct)
        {
            await chatMessageCache.Append(message, ct);
        }

        public async Task<ImmutableList<ChatMessage>> GetHistoryAsync(CancellationToken ct)
        {
            return await chatMessageCache.Get(ct);
        }
    }
}
