using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Middleware;
using Vehicle.Application.Interfaces;
using Vehicle.Infrastructure.Consumers;
using Vehicle.Infrastructure.Data;
using Vehicle.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

builder.Services.AddDbContext<VehicleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LocationUpdatedConsumer>();
    x.AddConsumer<BatteryStatusUpdatedConsumer>();
    x.AddConsumer<TripCompletedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<ICsvDataSeeder, CsvDataSeeder>();
builder.Services.AddScoped<Vehicle.Application.Interfaces.IServerService, Vehicle.Infrastructure.Services.ServerService>();
builder.Services.AddGrpc();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Title = "Vehicle API";
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
app.MapGrpcService<Vehicle.API.GrpcServices.VehicleGrpcService>();
app.MapControllers();
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<VehicleDbContext>();
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