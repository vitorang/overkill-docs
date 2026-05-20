using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;
using OverkillDocs.Api.Hubs;
using OverkillDocs.Core.DTOs.Document.Fragment;
using OverkillDocs.Core.Interfaces.Services;
namespace OverkillDocs.Api.Controllers;

[Route("api/document-fragments")]
[ApiController]
public class DocumentFragmentsController(IDocumentService documentService, IHubContext<MainHub> hubContext) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Create([FromBody] DocumentFragmentDto fragment, CancellationToken ct)
    {
        var result = await documentService.CreateFragment(fragment, ct);
        await NotifyDocumentChanged(result.DocumentHashId, ct);
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update([FromBody] DocumentFragmentDto fragment, CancellationToken ct)
    {
        await documentService.UpdateFragment(fragment, ct);
        await NotifyFragmentChanged(fragment, ct);
        return NoContent();
    }

    [HttpDelete("{hashId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(string hashId, CancellationToken ct)
    {
        string documentHashId = await documentService.GetDocumentHashIdByFragmentHashId(hashId, ct);
        await documentService.DeleteFragment(hashId, ct);
        await NotifyDocumentChanged(documentHashId, ct);
        return NoContent();
    }

    [HttpPost("{hashId}/lock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> LockFragment(string hashId, CancellationToken ct)
    {
        await documentService.LockFragment(hashId, ct);
        string documentHashId = await documentService.GetDocumentHashIdByFragmentHashId(hashId, ct);
        await NotifyActiveLocksChanged(documentHashId, ct);
        return NoContent();
    }

    [HttpDelete("{hashId}/lock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UnlockFragment(string hashId, CancellationToken ct)
    {
        await documentService.UnlockFragment(hashId, ct);
        string documentHashId = await documentService.GetDocumentHashIdByFragmentHashId(hashId, ct);
        await NotifyActiveLocksChanged(documentHashId, ct);
        return NoContent();
    }

    private async Task NotifyDocumentChanged(string documentHashId, CancellationToken ct)
    {
        var groupName = MainHub.DocumentViewerGetGroupName(documentHashId);
        await hubContext.Clients.Group(groupName)
            .SendAsync(HubEvents.DocumentViewer.DocumentChanged, documentHashId, ct);
    }

    private async Task NotifyFragmentChanged(DocumentFragmentDto fragment, CancellationToken ct)
    {
        var groupName = MainHub.DocumentViewerGetGroupName(fragment.DocumentHashId);
        await hubContext.Clients.Group(groupName)
            .SendAsync(HubEvents.DocumentViewer.FragmentChanged, fragment, ct);
    }

    private async Task NotifyActiveLocksChanged(string documentHashId, CancellationToken ct)
    {
        var activeLocks = await documentService.GetActiveLocks(documentHashId, ct);

        var groupName = MainHub.DocumentViewerGetGroupName(documentHashId);
        await hubContext.Clients.Group(groupName)
            .SendAsync(HubEvents.DocumentViewer.ActiveLocksChanged, activeLocks, ct);
    }
}
