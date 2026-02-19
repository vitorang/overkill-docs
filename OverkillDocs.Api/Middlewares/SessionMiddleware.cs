using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Api.Middlewares
{
    public class SessionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IUserSessionRepository sessionRepository, UserContext userContext)
        {
            var authSplit = (context.Request.Headers.Authorization.FirstOrDefault() ?? "").Split(" ");

            if (authSplit.Length == 2 && authSplit[0] == "Bearer")
            {
                var token = authSplit[1];
                var session = await sessionRepository.FindByTokenAsync(token, context.RequestAborted);

                if (session != null)
                {
                    userContext.UserId = session.UserId;
                    userContext.Username = session.User.Username;
                    userContext.Token = token;
                }
            }

            await next(context);
        }
    }
}
