using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Text;
using TaskManagement.Configurations;
using TaskManagement.Data;
using TaskManagement.Mapping;
using TaskManagement.Middleware;
using TaskManagement.Repositories;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utility;
using TaskManagement.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/taskmanagement-.txt", rollingInterval: RollingInterval.Day));

// Configure Mapster
MapsterConfig.ConfigureMapster();

// Add services to the container
builder.Services.AddControllers();

// Configure PostgreSQL with EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Register Repositories
builder.Services.AddScoped<IUserRepository, TaskManagement.Repositories.UserRepository>();
builder.Services.AddScoped<IProjectRepository, TaskManagement.Repositories.ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskManagement.Repositories.TaskRepository>();

// Register Services
builder.Services.AddScoped<IUserService, TaskManagement.Services.UserService>();
builder.Services.AddScoped<IProjectService, TaskManagement.Services.ProjectService>();
builder.Services.AddScoped<ITaskService, TaskManagement.Services.TaskService>();
builder.Services.AddScoped<IAuthService, TaskManagement.Services.AuthService>();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

// Configure JWT Authentication
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
    if (jwtOptions != null)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    }
});

builder.Services.AddAuthorization();

// Configure Redis
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();
    return ConnectionMultiplexer.Connect(redisOptions?.ConnectionString ?? "localhost:6379");
});
builder.Services.AddScoped<IRedisHelper, RedisHelper>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
// OpenAPI/Swagger configuration removed due to dependency issues

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Add global exception middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
