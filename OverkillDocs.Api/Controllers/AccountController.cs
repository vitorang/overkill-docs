using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Interfaces.Services;
using System.Collections.Immutable;

namespace OverkillDocs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public class AccountController(IAccountService accountService, IUserService userService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthRequestDto authDto, CancellationToken ct)
        {
            var result = await accountService.Login(authDto, ct);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await accountService.Logout(null, ct);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("Logout/{hashId}")]
        public async Task<IActionResult> Logout(string hashId, CancellationToken ct)
        {
            await accountService.Logout(hashId, ct);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] AuthRequestDto authDto, CancellationToken ct)
        {
            var result = await accountService.Register(authDto, ct);
            return Ok(result);
        }

        [HttpGet("Sessions")]
        [ProducesResponseType(typeof(ImmutableArray<UserSessionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponseDto>> Sessions(CancellationToken ct)
        {
            var result = await accountService.ListSessions(ct);
            return Ok(result);
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDto>> Profile(CancellationToken ct)
        {
            var result = await userService.GetProfile(ct: ct);
            return Ok(result);
        }

        [HttpPost("profile")]
        [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDto>> Profile([FromBody] ProfileDto profileDto, CancellationToken ct)
        {
            var result = await userService.UpdateProfile(profileDto, ct: ct);
            return Ok(result);
        }
    }
}
