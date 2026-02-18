using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.DTOs.Account;
using OverkillDocs.Core.Interfaces.Services;

namespace OverkillDocs.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        [HttpGet("Current")]
        public Task<IActionResult> Current(AuthRequestDto authDto)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] AuthRequestDto authDto)
        {
            var result = await accountService.LoginAsync(authDto);
            return Ok(result);
        }

        [HttpGet("Logout")]
        public Task<IActionResult> Logout(AuthRequestDto authDto)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpPost("Register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] AuthRequestDto authDto)
        {
            var result = await accountService.RegisterAsync(authDto);
            return Ok(result);
        }
    }
}
