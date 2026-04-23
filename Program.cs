using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyTextApi.Data;
using MyTextApi.Interfaces;
using MyTextApi.Services;
using Scalar.AspNetCore;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// HttpContextAccessor для CookieService
builder.Services.AddHttpContextAccessor();

// Добавляем сервисы в DI контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// PostgreSQL + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});


// JWT Authentication с поддержкой Cookie
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<JwtBearerOptions, CookieJwtBearerHandler>(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });

builder.Services.AddAuthorization();


// Регистрируем наш сервис (Singleton = один на все приложение)
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITextService, TextService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();

var app = builder.Build();

// Настраиваем middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI будет на /scalar
}


// CORS для разработки (важно для cookies с фронтенда)
if (app.Environment.IsDevelopment())
{
    app.UseCors(policy => policy
        .WithOrigins("http://localhost:3000", "http://localhost:5112")
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod());

    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();


// Важен порядок: CORS → Authentication → Authorization
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
