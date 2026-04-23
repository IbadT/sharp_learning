using MyTextApi.Models.Entities;

namespace MyTextApi.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiry();
    (string accessToken, string refreshToken, DateTime expiry) GenerateTokenPair(User user);
}