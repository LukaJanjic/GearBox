namespace API.Extensions;

public static class MiddlewarePipelineExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        app.MapControllers();
        return app;
    }
}
