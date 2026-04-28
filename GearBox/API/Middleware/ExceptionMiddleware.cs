using System.Net;
using System.Text.Json;
using Core.Exceptions;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                NotFoundException     => HttpStatusCode.NotFound,
                BadRequestException   => HttpStatusCode.BadRequest,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                ArgumentException     => HttpStatusCode.BadRequest,
                _                     => HttpStatusCode.InternalServerError
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var message = statusCode == HttpStatusCode.InternalServerError && !env.IsDevelopment()
                ? "An unexpected error occurred."
                : ex.Message;

            var details = env.IsDevelopment() && statusCode == HttpStatusCode.InternalServerError
                ? ex.StackTrace
                : null;

            var response = new { statusCode = context.Response.StatusCode, message, details };
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        }
    }
}
