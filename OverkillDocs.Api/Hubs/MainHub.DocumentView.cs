using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Core.DTOs.Document;

namespace OverkillDocs.Api.Hubs;

public partial class MainHub
{
    public const string documentViewGroup = "DocumentView";

    [HubMethodName($"{documentViewGroup}:GetActiveLocks")]
    public async Task<DocumentFragmentLockDto[]> DocumentViewGetActiveLocks(string documentHashId)
    {
        var activeLocks = await documentService.GetActiveLocks(documentHashId, default);
        return activeLocks;
    }

    public static string DocumentViewGetGroupName(string documentHashId)
    {
        return $"{documentViewGroup}:{documentHashId}";
    }
}
