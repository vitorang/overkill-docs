using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.DTOs.Users;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("me")]
        [ProducesResponseType(typeof(SimpleUserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleUserDto>> Current(CancellationToken ct)
        {
            var result = await userService.GetCurrent(ct);
            return Ok(result);
        }

        [HttpGet("{hashId}")]
        [ProducesResponseType(typeof(SimpleUserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleUserDto>> GetByHashId(string hashId, CancellationToken ct)
        {
            var result = await userService.GetByHashId(hashId, ct);
            return Ok(result);
        }
    }
}
