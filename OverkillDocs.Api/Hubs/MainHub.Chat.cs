using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;
using OverkillDocs.Core.DTOs.Chat;

namespace OverkillDocs.Api.Hubs
{
    public partial class MainHub
    {
        private const string chatGroup = "chat";

        [HubMethodName("Chat:Join")]
        public async Task JoinChat()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatGroup);
        }

        [HubMethodName("Chat:SendMessage")]
        public async Task SendMessage(string content)
        {
            var message = new ChatMessageDto(
                    Id: Ulid.NewUlid().ToString(),
                    Content: content,
                    UserHashId: hashids.Encode(userContext.UserId),
                    Timestamp: DateTime.UtcNow
                );

            await Clients.Group(chatGroup).SendAsync(HubEvents.Chat.MessageReceived, message);
        }
    }
}
