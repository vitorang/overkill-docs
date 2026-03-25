using OverkillDocs.Core.Entities.Chat;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;
using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class ChatRepository(IListCache<ChatMessage> chatMessageCache) : IChatRepository
    {
        public async Task AddRecentMessageAsync(ChatMessage message)
        {
            await chatMessageCache.Append(message);
        }

        public async Task<ImmutableArray<ChatMessage>> GetHistoryAsync()
        {
            return await chatMessageCache.Get();
        }
    }
}
