using System.Text.Json;

namespace Tixora.API.Middleware
{
    public class CustomAuthMiddleware
    {
    }
}
// Middleware/CustomAuthMiddleware.cs
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

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                message = "Admin can only access this resource"
            }));
        }
    }
}