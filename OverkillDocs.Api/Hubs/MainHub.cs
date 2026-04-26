using HashidsNet;
using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Api.Hubs;

public partial class MainHub(
    UserContext userContext,
    IHashids hashids,
    IChatService chatService
) : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
