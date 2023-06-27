using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Domain.Exceptions;
using Microsoft.Extensions.Localization;
using WebApi.Resources;

namespace WebApi.Middleware
{
    public class SampleAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
        private readonly IStringLocalizer<WebApiText> localizer;

        public SampleAuthorizationMiddlewareResultHandler(IStringLocalizer<WebApiText> localizer)
        {
            this.localizer = localizer;
        }

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if(authorizeResult.AuthorizationFailure == null
                && authorizeResult.Challenged == true
                && authorizeResult.Forbidden == false
                && authorizeResult.Succeeded == false)
            {
                throw new SampleUnauthorizedException(localizer["NotAuthenticated"].Value);
            }

            if(authorizeResult.AuthorizationFailure != null
                && authorizeResult.Challenged == false
                && authorizeResult.Forbidden == true
                && authorizeResult.Succeeded == false)
            {
                throw new SampleForbiddenException(localizer["Forbidden"].Value);
            }

            // Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
