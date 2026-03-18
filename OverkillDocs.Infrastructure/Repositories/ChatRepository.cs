using OverkillDocs.Core.Entities.Chat;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Interfaces.Repositories;

namespace OverkillDocs.Infrastructure.Repositories
{
    public class ChatRepository(IAppCache<ChatHistory> chatHistoryCache) : IChatRepository
    {
        private static readonly SemaphoreSlim semaphore = new(1, 1);

        public async Task AddRecentMessageAsync(ChatMessage message, CancellationToken ct)
        {
            await semaphore.WaitAsync(ct);
            try
            {
                var history = await GetHistoryOrDefaultAsync(ct);
                history.Add(message);
                await chatHistoryCache.Set(history, ct);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<ChatHistory> GetHistoryAsync(CancellationToken ct)
        {
            await semaphore.WaitAsync(ct);
            try
            {
                return await GetHistoryOrDefaultAsync(ct);
            }
            finally
            {
                semaphore.Release(); 
            }
        }

        private async Task<ChatHistory> GetHistoryOrDefaultAsync(CancellationToken ct)
        {
            return await chatHistoryCache.Get(ChatHistory.DefaultId, ct) ?? new ChatHistory();
        }
    }
}
