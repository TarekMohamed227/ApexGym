using ApexGym.API.Middleware;
using ApexGym.Application.Dtos.Validators;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Application.Mappings;
using ApexGym.Infrastructure.Data;
using ApexGym.Infrastructure.Data.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register our AppDbContext with the Dependency Injection container
// This is where we configure the connection to SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the Repository for Dependency Injection
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

// Register AutoMapper
// This line scans the entire assembly (ApexGym.Application) for all Profile classes
// and configures AutoMapper with them.
builder.Services.AddAutoMapper(typeof(MemberProfile)); // You can use any type from the assembly

// Add FluentValidation services
builder.Services.AddValidatorsFromAssemblyContaining<MemberUpdateDtoValidator>(); // Scans the assembly for all Validators


var app = builder.Build();

// ... other services (AddDbContext, AddScoped, AddAutoMapper) ...
// Add our custom exception middleware at the top of the pipeline
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();