using OverkillDocs.Core.DTOs.Chat;
using System.Collections.Immutable;

namespace OverkillDocs.Core.Interfaces.Services;

public interface IChatService
{
    Task AddRecent(ChatMessageDto messageDto);
    Task<ImmutableList<ChatMessageDto>> GetRecent();
}
