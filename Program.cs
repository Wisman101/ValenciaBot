using Microsoft.EntityFrameworkCore;
using ValenciaBot.Data;
using ValenciaBot.Data.SeedData;
using ValenciaBot.Startup;

var builder = WebApplication.CreateBuilder(args);

if(builder.Environment.EnvironmentName == "Development")
{
    builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    builder.Services.AddDbContext<MainContext>(
            o => o.UseNpgsql(builder.Configuration.GetConnectionString("MainContext"))
        );
}
else
{
    builder.Configuration.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);
    Console.Write(builder.Configuration.GetConnectionString("POSTGRESQLCONNSTR_MainContext"));
    builder.Services.AddDbContext<MainContext>(
            o => o.UseNpgsql(builder.Configuration.GetConnectionString("POSTGRESQLCONNSTR_MainContext"))
        );
}

builder.Services.RegisterServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MainContext>();
    db.Database.Migrate();
    await SeedData.Execute(db, app.Services.GetRequiredService<IWebHostEnvironment>());
}

app.ConfigureMiddleware();

app.Run();