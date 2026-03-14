using Microsoft.AspNetCore.SignalR;

namespace OverkillDocs.Api.Hubs
{
    public abstract partial class MainHub : Hub
    {
        private const string chatGroup = "chat";

        public async Task JoinChat()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatGroup);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveCurrentEditor();
        }

        protected abstract Task RemoveCurrentEditor();
    }
}
