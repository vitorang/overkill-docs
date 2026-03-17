using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OverkillDocs.Core.Security;

public class UserContext(IHttpContextAccessor accessor)
{
    private static readonly UserIdentity Anonymous = new(0, "Anonymous", string.Empty);

    public UserIdentity Identity => GetIdentity();

    public int UserId => Identity.UserId;
    public string Username => Identity.Username;
    public string Token => Identity.Token;
    public bool IsAuthorized => Identity != Anonymous;

    private UserIdentity GetIdentity()
    {
        var user = accessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return Anonymous;

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var usernameClaim = user.FindFirst(ClaimTypes.Name)?.Value;

        if (!int.TryParse(userIdClaim, out var userId) || string.IsNullOrWhiteSpace(usernameClaim))
            return Anonymous;

        return new UserIdentity(
            UserId: userId,
            Username: usernameClaim,
            Token: ExtractToken()
        );
    }

    private string ExtractToken()
    {
        var context = accessor.HttpContext;
        if (context == null) return string.Empty;

        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return authHeader["Bearer ".Length..].Trim();

        return context.Request.Query["auth_token"].FirstOrDefault() ?? string.Empty;
    }

    public record UserIdentity(int UserId, string Username, string Token);
}