using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;

namespace OverkillDocs.Api.Hubs;

public partial class MainHub
{
    public const string documentViewerGroup = "DocumentViewer";

    [HubMethodName($"{documentViewerGroup}:RequestActiveLocks")]
    public async Task DocumentViewerRequestActiveLocks(string documentHashId)
    {
        var activeLocks = await documentService.GetActiveLocks(documentHashId, default);
        await Clients.Caller.SendAsync(HubEvents.DocumentViewer.ActiveLocksChanged, activeLocks);
    }

    [HubMethodName($"{documentViewerGroup}:Join")]
    public async Task DocumentViewerJoin(string documentHashId)
    {
        var group = DocumentViewerGetGroupName(documentHashId);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }


    [HubMethodName($"{documentViewerGroup}:Leave")]
    public async Task DocumentViewerLeave(string documentHashId)
    {
        var group = DocumentViewerGetGroupName(documentHashId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }

    public static string DocumentViewerGetGroupName(string documentHashId)
    {
        return $"{documentViewerGroup}:{documentHashId}";
    }
}
