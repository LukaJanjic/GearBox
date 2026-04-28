using API.Middleware;

namespace API.Extensions;

public static class MiddlewarePipelineExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseCors("CorsPolicy");
        app.UseHttpsRedirection();
        app.MapControllers();
        return app;
    }
}
