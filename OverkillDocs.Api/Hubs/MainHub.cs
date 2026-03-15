using HashidsNet;
using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Api.Hubs
{
    public partial class MainHub(
        UserContext userContext,
        Hashids hashids,
        IDocumentStateCache documentStateCache,
        IEditorStateCache editorStateCache
    ) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await JoinChat();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveCurrentEditor();
            await base.OnDisconnectedAsync(exception);
        }
    }
}
