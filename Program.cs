using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Application.Services;
using CompatibleAPI.Infrastructure.Persistence;
using CompatibleAPI.Infrastructure.Middleware;
using CompatibleAPI.Infrastructure.Persistence.Repositories;
using CompatibleAPI.Hubs;
using Amazon.S3;

// Load environment variables from .env file
Env.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback to environment variables for production
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "compa";
    var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
    
    connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPassword}";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure AWS S3
builder.Services.AddAWSService<IAmazonS3>();

// Register DDD services
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Add SignalR with JSON configuration
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
})
.AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// Configure CORS
if (builder.Environment.IsDevelopment())
{
    // For development, allow any origin
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
}
else
{
    // For production, use specific origins
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp",
            builder => builder
                .WithOrigins(
                    "http://localhost:8081", 
                    "http://localhost:19006", 
                    "exp://localhost:19000",
                    Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });
}

WebApplication app = builder.Build();

// Seed the database (only if database is available)
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        await DataSeeder.SeedData(context, logger);
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database seeding failed - this is normal if database is not available. Application will continue to run.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Remove HTTPS redirection for local development
// app.UseHttpsRedirection();

app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowReactApp");

// Add error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Map SignalR hub
app.MapHub<MessageHub>("/messageHub");

app.Run();
