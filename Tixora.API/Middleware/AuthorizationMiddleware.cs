
using Tixora.Service.Exceptions;

namespace Tixora.API.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Path.StartsWithSegments("/api/user/login") ||
                context.Request.Path.StartsWithSegments("/api/user/register"))
            {
                await _next(context);
                return;
            }
            var endpoint = context.GetEndpoint();
            if(endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null)
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    throw new UnauthorizedException("User is not authenticated");
                }
            }
            await _next(context);
        }
    }
}
