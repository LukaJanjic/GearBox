using API.Extensions;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseApplicationMiddleware();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GearBoxContext>();
    await context.Database.MigrateAsync();   // kreira sve tabele (i Identity)
    await RoleSeeder.SeedAsync(app.Services); // tek sad kad tabele postoje
    await SeedDataService.SeedAsync(context);
}

app.Run();
