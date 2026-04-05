using Alert.Application.Interfaces;
using Alert.Infrastructure.Consumers;
using Alert.Infrastructure.Data;
using Alert.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

builder.Services.AddDbContext<AlertDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TelemetryAlertConsumer>();

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
builder.Services.AddScoped<Alert.Application.Interfaces.IEmailService, Alert.Infrastructure.Services.EmailService>();
builder.Services.AddGrpc();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Title = "Alert API";
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
app.MapGrpcService<Alert.API.GrpcServices.AlertGrpcService>();
app.MapControllers();
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<AlertDbContext>();
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