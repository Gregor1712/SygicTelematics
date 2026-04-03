using Microsoft.EntityFrameworkCore;
using TestProject.Interfaces;
using TestProject.Mapper;
using TestProject.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using NSwag;
using NSwag.Generation.Processors.Security;
using TestProject.Data;
using TestProject.Entities;
using NLog;
using NLog.Web;
using TestProject.Middleware;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICsvDataSeeder, CsvDataSeeder>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICpuService, CpuService>();
builder.Services.AddScoped<IServerService, ServerService>();
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
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
        document.Info.Title = "TestProject API";
        document.Info.Version = "v1";
    };
    
    config.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT", 
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });

    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});


builder.Services.AddIdentityCore<AppUser>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
        opt.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"]
                       ?? throw new Exception("Token key not found - Program.cs");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("Admin", "User"));

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
app.UseHttpsRedirection();
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await context.Database.MigrateAsync();
    await SeedUsers.SeedUsersData(userManager, roleManager);
}
catch (Exception ex)
{
    logger.Error(ex, "An error occurred during migration");
}

app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}


// ctrl+shift+/

/*# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Add migration
dotnet ef migrations add InitialCreate

# Update database
    dotnet ef database update

# If you need to remove the last migration
    dotnet ef migrations remove*/
    
    
// dotnet ef migrations add JwtAuthTables   // creates migration files
// dotnet ef database update                 // applies them
    