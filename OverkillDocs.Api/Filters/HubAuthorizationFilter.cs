using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Api.Filters
{
    public class HubAuthorizationFilter(UserContext userContext) : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            if (!userContext.IsAuthorized)
            {
                invocationContext.Context.Abort();
                return null;
            }

            return await next(invocationContext);
        }
    }
}