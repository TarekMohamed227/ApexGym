using ApexGym.API.Filters;
using ApexGym.API.Middleware;
using ApexGym.Application.Dtos.Validators;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Application.Mappings;
using ApexGym.Infrastructure.Data;
using ApexGym.Infrastructure.Data.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// Create and configure the Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set the minimum level to log
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information) // Quiet down Microsoft's internal logs
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning) // Only log EF warnings and errors
    .Enrich.FromLogContext() // Adds context information like ThreadId
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "logs/log-.txt", // Log file path. It will create new files based on the date.
        rollingInterval: RollingInterval.Day, // Create a new file each day
        restrictedToMinimumLevel: LogEventLevel.Information, // Only log Info and above to file
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    Log.Information("Starting ApexGym web application...");

    var builder = WebApplication.CreateBuilder(args);

    // CLEAR all existing logging providers and ADD Serilog
    builder.Logging.ClearProviders(); // Remove the default console logger
    builder.Host.UseSerilog(); // Tell ASP.NET Core to use Serilog for all logging

    // ===== SERVICE REGISTRATION =====
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IMemberRepository, MemberRepository>();
    builder.Services.AddAutoMapper(typeof(MemberProfile));
    builder.Services.AddValidatorsFromAssemblyContaining<MemberUpdateDtoValidator>();

    // ===== END OF SERVICE REGISTRATION =====

    var app = builder.Build();

    // ===== MIDDLEWARE PIPELINE CONFIGURATION =====
    app.UseMiddleware<ExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Application configured successfully. Starting now...");
    app.Run();
}
catch (Exception ex)
{
    // This will catch any exceptions that happen during application startup
    Log.Fatal(ex, "Application startup failed!");
}
finally
{
    // This ensures any buffered log messages are written before the application closes
    Log.CloseAndFlush();
}