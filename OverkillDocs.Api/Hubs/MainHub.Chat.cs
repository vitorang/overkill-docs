using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;
using OverkillDocs.Core.DTOs.Chat;

namespace OverkillDocs.Api.Hubs
{
    public partial class MainHub
    {
        private const string chatGroup = "chat";

        private async Task JoinChat()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatGroup);
        }

        public async Task SendMessage(string content)
        {
            var message = new ChatMessageDto(
                    Content: content,
                    UserHashId: hashids.Encode(userContext.UserId),
                    Timestamp: DateTime.UtcNow
                );

            await Clients.Group(chatGroup).SendAsync(HubEvents.Chat.MessageReceived, message);
        }
    }
}
