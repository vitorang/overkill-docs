using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Api.Filters
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(em => em is AllowAnonymousAttribute);

            if (hasAllowAnonymous) return;

            var userContext = context.HttpContext.RequestServices.GetRequiredService<UserContext>();

            if (!userContext.IsAuthorized)
                throw new UnauthorizedException("Sessão inválida ou expirada.");
        }
    }
}
