namespace ValenciaBot.Startup;

public static class DependencyInjectionSetup
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthorization();
        services.AddControllers().AddNewtonsoftJson();
        services.AddAutoMapper(typeof(Program));

        return services;
    }
}
