using FlashcardApp.Core.Auth.Services;
using FlashcardApp.Infrastructure.Data;
using FlashcardApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using FlashcardApp.Core.MappingProfiles;
using FlashcardApp.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFlashcardCategoryRepository, FlashcardCategoryRepository>();
builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();
// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(AuthProfile)); // U¿yj typu profilu

// Configure JWT authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// 1. Dodaj CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:8081") // adres Twojego RN dev servera
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthorization();

// Add OpenAPI (Swagger)
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors("AllowFrontend");
// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();