using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyTextApi.Data;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;
using MyTextApi.Models.Entities;

namespace MyTextApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    public readonly IPasswordService _passwordService;
    public readonly IJwtService _jwtService;

    public AuthService(
        AppDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService
    )
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return null; // Пользователь с таким email уже существует
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            AccessTokenExpiry = _jwtService.GetAccessTokenExpiry(),
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return null;
        }

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
        {
            return null; // Неверный пароль
        }

        return new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            AccessTokenExpiry = _jwtService.GetAccessTokenExpiry(),
        };
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        // В реальном приложении здесь проверка refresh token из хранилища (Redis)
        // Но по условию задачи refresh token в cookie, без хранения в БД
        // Для простоты пропускаем валидацию refresh token (в production добавь Redis)

        // Декодируем refresh token чтобы получить userId (если JWT) 
        // или используй отдельное хранилище сессий

        // Упрощённый вариант — refresh token не валидируем, просто выдаём новую пару
        // В production: проверь подпись/хранилище

        return null; // Заглушка — в production реализуй валидацию
    }

    public async Task<MeResponse?> GetMeAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new MeResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }

}