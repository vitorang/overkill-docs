using OverkillDocs.Core.Interfaces.Repositories;
using System.Security.Claims;

namespace OverkillDocs.Api.Middlewares
{
    public class SessionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IUserSessionRepository sessionRepository)
        {
            string token = context.Request.Path.StartsWithSegments("/hubs")
                ? GetTokenFromQuery(context)
                : GetTokenFromHeader(context);

            if (!string.IsNullOrWhiteSpace(token))
            {
                var session = await sessionRepository.FindIdentityByTokenAsync(token, context.RequestAborted);

                if (session != null)
                {
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
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader)) return "";

            var authSplit = authHeader.Split(" ");
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