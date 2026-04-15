using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("me")]
        [ProducesResponseType(typeof(SimpleUserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleUserDto>> Current(CancellationToken ct)
        {
            var result = await userService.GetCurrent(ct: ct);
            return Ok(result);
        }

        [HttpGet("{hashId}")]
        [ProducesResponseType(typeof(SimpleUserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleUserDto>> GetByHashId(string hashId, CancellationToken ct)
        {
            var result = await userService.GetByHashId(hashId, ct: ct);
            return Ok(result);
        }
    }
}
