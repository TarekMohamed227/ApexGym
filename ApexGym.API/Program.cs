using System.Text;
using ApexGym.API.Filters;
using ApexGym.API.Middleware;
using ApexGym.Application.Dtos.Validators;
using ApexGym.Application.Interfaces;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Application.Mappings;
using ApexGym.Domain.Entities;
using ApexGym.Infrastructure.Data;
using ApexGym.Infrastructure.Data.Repositories;

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

// --------------------
// Setup Serilog Logger
// --------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    Log.Information("Starting ApexGym web application...");

    var builder = WebApplication.CreateBuilder(args);

    // Clear default logging providers and add Serilog
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog();

    // ----------------------
    // Add Services to DI
    // ----------------------
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // ----------------------
    // Configure CORS
    // ----------------------
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularDev", policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular dev URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Optional if sending cookies
        });
    });

    // ----------------------
    // Database Context
    // ----------------------
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // ----------------------
    // Application Services
    // ----------------------
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IMemberRepository, MemberRepository>();
    builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
    builder.Services.AddScoped<IWorkoutClassRepository, WorkoutClassRepository>();
    builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
    builder.Services.AddAutoMapper(typeof(MemberProfile));
    builder.Services.AddValidatorsFromAssemblyContaining<MemberUpdateDtoValidator>();
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddMediatR(typeof(Program));


    // ----------------------
    // Identity Configuration
    // ----------------------
    builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // ----------------------
    // JWT Authentication
    // ----------------------
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
            ),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddScoped<ITokenService, TokenService>();

    var app = builder.Build();

    // ----------------------
    // Use CORS before routing
    // ----------------------
    app.UseCors("AllowAngularDev");

    // ----------------------
    // Middleware pipeline
    // ----------------------
    app.UseMiddleware<ExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // ----------------------
    // Initialize Roles
    // ----------------------
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var roles = new[] { "Admin", "Member" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
    }

    Log.Information("Application configured successfully. Starting now...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed!");
}
finally
{
    Log.CloseAndFlush();
}
