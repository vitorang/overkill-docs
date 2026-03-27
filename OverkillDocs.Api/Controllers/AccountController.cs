using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthRequestDto authDto, CancellationToken ct)
        {
            var result = await accountService.LoginAsync(authDto, ct);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await accountService.LogoutAsync(ct);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] AuthRequestDto authDto, CancellationToken ct)
        {
            var result = await accountService.RegisterAsync(authDto, ct);
            return Ok(result);
        }
    }
}
