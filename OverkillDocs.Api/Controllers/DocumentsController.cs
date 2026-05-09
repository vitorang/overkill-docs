using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController(IDocumentService documentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(DocumentDto[]), StatusCodes.Status200OK)]
    public async Task<ActionResult<SimpleUserDto>> Search(CancellationToken ct)
    {
        var result = await documentService.List(ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SimpleUserDto>> Create([FromBody] DocumentDto document, CancellationToken ct)
    {
        var result = await documentService.Create(document, ct);
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SimpleUserDto>> Update([FromBody] DocumentDto document, CancellationToken ct)
    {
        var result = await documentService.Update(document, ct);
        return Ok(result);
    }

    [HttpDelete("{hashId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<SimpleUserDto>> Delete(string hashId, CancellationToken ct)
    {
        await documentService.Delete(hashId, ct);
        return NoContent();
    }
}
