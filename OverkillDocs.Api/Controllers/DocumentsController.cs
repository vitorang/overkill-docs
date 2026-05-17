using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;
using OverkillDocs.Api.Hubs;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController(IDocumentService documentService, IHubContext<MainHub> hubContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(DocumentSummaryDto[]), StatusCodes.Status200OK)]
    public async Task<ActionResult<SimpleUserDto>> List(CancellationToken ct)
    {
        var result = await documentService.List(ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DocumentSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SimpleUserDto>> Create([FromBody] DocumentSummaryDto document, CancellationToken ct)
    {
        var result = await documentService.Create(document, ct);
        await NotifyDocumentIndexChanged(ct);
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SimpleUserDto>> Update([FromBody] DocumentSummaryDto document, CancellationToken ct)
    {
        await documentService.Update(document, ct);
        await NotifyDocumentIndexChanged(ct);
        return NoContent();
    }

    [HttpDelete("{hashId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<SimpleUserDto>> Delete(string hashId, CancellationToken ct)
    {
        await documentService.Delete(hashId, ct);
        await NotifyDocumentIndexChanged(ct);
        return NoContent();
    }

    private async Task NotifyDocumentIndexChanged(CancellationToken ct)
    {
        await hubContext.Clients.Group(MainHub.documentIndexGroup)
            .SendAsync(HubEvents.DocumentIndex.Changed, ct);
    }
}
