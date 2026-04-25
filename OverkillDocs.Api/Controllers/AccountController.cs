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
        [HttpPost("Change-Password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeDto passwordChangeDto, CancellationToken ct)
        {
            await accountService.ChangePassword(passwordChangeDto, ct);
            return NoContent();
        }

        [HttpPost("Delete-Account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAccount([FromBody] AccountDeletionDto accountDeletionDto, CancellationToken ct)
        {
            await accountService.AnonymizeAccount(accountDeletionDto, ct);
            return NoContent();
        }

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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await accountService.Logout(null, ct);
            return NoContent();
        }

        [HttpPost("Logout/{hashId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
