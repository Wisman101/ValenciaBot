using Microsoft.EntityFrameworkCore;
using ValenciaBot.Data;
using ValenciaBot.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddDbContext<MainContext>(
            o => o.UseNpgsql(builder.Configuration.GetConnectionString("MainContext"))
        );

builder.Services.RegisterServices(); 

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MainContext>();
    db.Database.Migrate();
}

app.ConfigureMiddleware();

app.Run();