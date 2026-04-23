using MyTextApi.Models.DTOs;

namespace MyTextApi.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
    Task<MeResponse?> GetMeAsync(int userId);
}