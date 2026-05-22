using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.Attributes;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ProducesErrorResponseType(typeof(ProblemDetails))]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("Me")]
    [ProducesStatusFor]
    public async Task<ActionResult<SimpleUserDto>> Current(CancellationToken ct)
    {
        var result = await userService.GetCurrent(ct: ct);
        return Ok(result);
    }

    [HttpGet("{hashId}")]
    [ProducesStatusFor(typeof(NotFoundException))]
    public async Task<ActionResult<SimpleUserDto>> GetByHashId(string hashId, CancellationToken ct)
    {
        var result = await userService.GetByHashId(hashId, ct: ct);
        return Ok(result);
    }
}
