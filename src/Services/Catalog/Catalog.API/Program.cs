using Catalog.Application.Interfaces;
using Catalog.Application.Mapper;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICsvDataSeeder, CsvDataSeeder>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICpuService, CpuService>();
builder.Services.AddScoped<IServerService, ServerService>();
builder.Services.AddCors();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Title = "Catalog API";
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
app.MapControllers();
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<CatalogDbContext>();
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