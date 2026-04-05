using Battery.Application.Interfaces;
using Battery.Infrastructure.Data;
using Battery.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

builder.Services.AddDbContext<BatteryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });
    });
});

builder.Services.AddScoped<ICsvDataSeeder, CsvDataSeeder>();
builder.Services.AddGrpc();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Title = "Battery API";
        document.Info.Version = "v1";
    };
});

builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.MapGrpcService<Battery.API.GrpcServices.BatteryGrpcService>();
app.MapControllers();
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<BatteryDbContext>();
    await context.Database.MigrateAsync();

    var seeder = services.GetRequiredService<ICsvDataSeeder>();
    await seeder.SeedDataAsync();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration/seeding");
}

app.Run();