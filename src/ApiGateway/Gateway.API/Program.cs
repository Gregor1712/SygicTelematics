using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Kernel.Middleware;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddHttpClient("IdentityService", client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Identity"]
//         ?? "http://localhost:5101");
// });

builder.Services.AddHttpClient("CatalogService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Catalog"]
        ?? "http://localhost:5102");
});

builder.Services.AddHttpClient("VehicleService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Vehicle"]
        ?? "http://localhost:5103");
});

builder.Services.AddHttpClient("LocationService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Location"]
        ?? "http://localhost:5104");
});

builder.Services.AddHttpClient("BatteryService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Battery"]
        ?? "http://localhost:5105");
});

builder.Services.AddHttpClient("TripService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Trip"]
        ?? "http://localhost:5106");
});

builder.Services.AddHttpClient("TelemetryService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Telemetry"]
        ?? "http://localhost:5107");
});

builder.Services.AddHttpClient("AlertService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Alert"]
        ?? "http://localhost:5108");
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"]
                       ?? throw new Exception("Token key not found");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Title = "BFF Gateway API";
        document.Info.Version = "v1";
    };
});

builder.Services.AddCors();
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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();