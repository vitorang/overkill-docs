using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;
using OverkillDocs.Core.DTOs.Chat;

namespace OverkillDocs.Api.Hubs
{
    public partial class MainHub
    {
        private const string chatGroup = "Chat";

        [HubMethodName("Chat:Join")]
        public async Task ChatJoin()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatGroup);
        }

        [HubMethodName("Chat:SendMessage")]
        public async Task ChatSendMessage(string content)
        {
            var message = new ChatMessageDto(
                    Id: Ulid.NewUlid().ToString(),
                    Content: content,
                    UserHashId: hashids.Encode(userContext.UserId),
                    Timestamp: DateTime.UtcNow
                );

            await chatService.AddRecent(message, default);
            await Clients.Group(chatGroup).SendAsync(HubEvents.Chat.MessageReceived, message);
        }

        [HubMethodName("Chat:RequestRecentMessages")]
        public async Task RequestRecentMessages()
        {
            var messages = await chatService.GetRecent(default);
            await Clients.Caller.SendAsync(HubEvents.Chat.RecentMessagesReceived, messages);
        }
    }
}
