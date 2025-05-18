using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Tixora.API.Attributes; // Add this line

namespace Tixora.API.Middleware
{
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = "Please authenticate to access this resource"
                }));
            }

            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                var endpoint = context.GetEndpoint();
                var isAdminEndpoint = endpoint?.Metadata.GetMetadata<AdminOnlyAttribute>() != null;

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = isAdminEndpoint
                        ? "Admin privileges required to access this resource"
                        : "You don't have permission to access this resource"
                }));
            }
        }
    }
}