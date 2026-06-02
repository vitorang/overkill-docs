using Microsoft.AspNetCore.SignalR;

namespace OverkillDocs.Api.Hubs;

public partial class MainHub
{
    public const string documentIndexGroup = "DocumentIndex";

    [HubMethodName($"{documentIndexGroup}:Join")]
    public async Task DocumentIndexJoin()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, documentIndexGroup);
    }
}
