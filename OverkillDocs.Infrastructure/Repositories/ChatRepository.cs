using OverkillDocs.Core.Entities.Chat;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Infrastructure.Interfaces;
using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Repositories;

internal sealed class ChatRepository(IListCache<ChatMessage> chatMessageCache) : IChatRepository
{
    public async Task AddRecentMessage(ChatMessage message)
    {
        await chatMessageCache.Append(message);
    }

    public async Task<ImmutableArray<ChatMessage>> GetHistory()
    {
        return await chatMessageCache.Get();
    }
}
