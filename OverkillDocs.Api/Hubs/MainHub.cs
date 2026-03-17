using HashidsNet;
using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Security;
using OverkillDocs.Core.States;

namespace OverkillDocs.Api.Hubs
{
    public partial class MainHub(
        UserContext userContext,
        IHashids hashids,
        IAppCache<DocumentState> documentStateCache,
        IAppCache<EditorState> editorStateCache
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
