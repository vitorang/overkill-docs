using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Security;
using System.Security.Claims;

namespace OverkillDocs.Api.Middlewares
{
    public class SessionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IUserSessionRepository sessionRepository, UserContext userContext)
        {
            string token = context.Request.Path.StartsWithSegments("/hubs")
                ? GetTokenFromQuery(context)
                : GetTokenFromHeader(context);

            if (!string.IsNullOrWhiteSpace(token))
            {
                var session = await sessionRepository.FindIdentityByTokenAsync(token, context.RequestAborted);

                if (session != null)
                {
                    userContext.Identity = new(
                        UserId: session.UserId,
                        Username: session.Username,
                        Token: userContext.Token
                    );

                    var claims = new[] {
                        new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                        new Claim(ClaimTypes.Name, session.Username)
                    };

                    var identity = new ClaimsIdentity(claims, "ManualAuth");
                    context.User = new ClaimsPrincipal(identity);
                }
            }

            await next(context);
        }

        private static string GetTokenFromHeader(HttpContext context)
        {
            var authSplit = (context.Request.Headers.Authorization.FirstOrDefault() ?? "").Split(" ");
            if (authSplit.Length == 2 && authSplit[0] == "Bearer")
                return authSplit[1];

            return "";
        }

        private static string GetTokenFromQuery(HttpContext context)
        {
            return context.Request.Query["auth_token"].FirstOrDefault() ?? "";
        }
    }
}
