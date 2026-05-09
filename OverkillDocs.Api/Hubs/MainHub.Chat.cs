using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;
using OverkillDocs.Core.DTOs.Chat;

namespace OverkillDocs.Api.Hubs;

public partial class MainHub
{
    public const string chatGroup = "Chat";

    [HubMethodName($"{chatGroup}:Join")]
    public async Task ChatJoin()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatGroup);
    }

    [HubMethodName($"{chatGroup}:SendMessage")]
    public async Task ChatSendMessage(string content)
    {
        var message = new ChatMessageDto(
                Id: Ulid.NewUlid().ToString(),
                Content: content,
                UserHashId: hashids.Encode(userContext.UserId),
                Timestamp: DateTime.UtcNow
            );

        await chatService.AddRecent(message);
        await Clients.Group(chatGroup).SendAsync(HubEvents.Chat.MessageReceived, message);
    }

    [HubMethodName($"{chatGroup}:RequestRecentMessages")]
    public async Task RequestRecentMessages()
    {
        var messages = await chatService.GetRecent();
        await Clients.Caller.SendAsync(HubEvents.Chat.RecentMessagesReceived, messages);
    }
}
