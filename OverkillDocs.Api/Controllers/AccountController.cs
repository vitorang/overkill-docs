using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.Attributes;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesErrorResponseType(typeof(ProblemDetails))]
public class AccountController(IAccountService accountService, IUserService userService) : ControllerBase
{
    [HttpPost("ChangePassword")]
    [ProducesStatusFor]
    public async Task<NoContentResult> ChangePassword([FromBody] PasswordChangeDto passwordChangeDto, CancellationToken ct)
    {
        await accountService.ChangePassword(passwordChangeDto, ct);
        return NoContent();
    }

    [HttpPost("DeleteAccount")]
    [ProducesStatusFor]
    public async Task<NoContentResult> DeleteAccount([FromBody] AccountDeletionDto accountDeletionDto, CancellationToken ct)
    {
        await accountService.AnonymizeAccount(accountDeletionDto, ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [ProducesStatusFor(typeof(NotFoundException))]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthRequestDto authDto, CancellationToken ct)
    {
        var result = await accountService.Login(authDto, ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("Logout")]
    [ProducesStatusFor(typeof(NotFoundException), typeof(ForbiddenException))]
    public async Task<NoContentResult> Logout(CancellationToken ct)
    {
        await accountService.Logout(null, ct);
        return NoContent();
    }

    [HttpPost("Logout/{hashId}")]
    [ProducesStatusFor(typeof(NotFoundException), typeof(ForbiddenException))]
    public async Task<NoContentResult> Logout(string hashId, CancellationToken ct)
    {
        await accountService.Logout(hashId, ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    [ProducesStatusFor(typeof(ConflictException))]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] AuthRequestDto authDto, CancellationToken ct)
    {
        var result = await accountService.Register(authDto, ct);
        return Ok(result);
    }

    [HttpGet("Sessions")]
    [ProducesStatusFor]
    public async Task<ActionResult<AuthResponseDto>> Sessions(CancellationToken ct)
    {
        var result = await accountService.ListSessions(ct);
        return Ok(result);
    }

    [HttpGet("Profile")]
    [ProducesStatusFor]
    public async Task<ActionResult<ProfileDto>> Profile(CancellationToken ct)
    {
        var result = await userService.GetProfile(ct: ct);
        return Ok(result);
    }

    [HttpPut("Profile")]
    [ProducesStatusFor(typeof(ForbiddenException))]
    public async Task<ActionResult<ProfileDto>> Profile([FromBody] ProfileDto profileDto, CancellationToken ct)
    {
        var result = await userService.UpdateProfile(profileDto, ct: ct);
        return Ok(result);
    }
}
