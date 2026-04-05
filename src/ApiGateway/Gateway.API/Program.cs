using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Grpc.Alert;
using Shared.Grpc.Battery;
using Shared.Grpc.Location;
using Shared.Grpc.Telemetry;
using Shared.Grpc.Trip;
using Shared.Grpc.Vehicle;
using Shared.Kernel.Middleware;

var builder = WebApplication.CreateBuilder(args);

// HttpClient for Identity (cookie/header forwarding — not suitable for gRPC)
builder.Services.AddHttpClient("IdentityService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Identity"]
        ?? "http://localhost:5101");
});

// HttpClient for Vehicle GetAll (complex query-string filter forwarding)
builder.Services.AddHttpClient("VehicleService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GrpcUrls:Vehicle"]
        ?? "http://localhost:5103");
});

// gRPC clients for internal service-to-service calls (low latency)
builder.Services.AddGrpcClient<VehicleGrpc.VehicleGrpcClient>(o =>
    o.Address = new Uri(builder.Configuration["GrpcUrls:Vehicle"] ?? "http://localhost:5103"));

builder.Services.AddGrpcClient<LocationGrpc.LocationGrpcClient>(o =>
    o.Address = new Uri(builder.Configuration["GrpcUrls:Location"] ?? "http://localhost:5104"));

builder.Services.AddGrpcClient<BatteryGrpc.BatteryGrpcClient>(o =>
    o.Address = new Uri(builder.Configuration["GrpcUrls:Battery"] ?? "http://localhost:5105"));

builder.Services.AddGrpcClient<TripGrpc.TripGrpcClient>(o =>
    o.Address = new Uri(builder.Configuration["GrpcUrls:Trip"] ?? "http://localhost:5106"));

builder.Services.AddGrpcClient<TelemetryGrpc.TelemetryGrpcClient>(o =>
    o.Address = new Uri(builder.Configuration["GrpcUrls:Telemetry"] ?? "http://localhost:5107"));

builder.Services.AddGrpcClient<AlertGrpc.AlertGrpcClient>(o =>
    o.Address = new Uri(builder.Configuration["GrpcUrls:Alert"] ?? "http://localhost:5108"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"]
                       ?? throw new Exception("Token key not found");
        options.UseSecurityTokenValidators = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireAuthenticatedUser());
});
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

// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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
app.MapReverseProxy();
app.MapHealthChecks("/health");

app.Run();
