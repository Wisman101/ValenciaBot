using Microsoft.AspNetCore.Cors;
namespace ValenciaBot.Startup;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.UseCors("AllowAllOrigins");

        return app;
    }
}