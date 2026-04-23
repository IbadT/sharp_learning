using MyTextApi.Interfaces;

namespace MyTextApi.Services;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string AccessTokenName = "access_token";
    private const string RefreshTokenName = "refresh_token";

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetAccessToken(string token, DateTime expiry)
    {
        var context = _httpContextAccessor.HttpContext!;
        context.Response.Cookies.Append(AccessTokenName, token, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = expiry,
        });
    }

    public void SetRefreshToken(string token)
    {
        var context = _httpContextAccessor.HttpContext!;
        context.Response.Cookies.Append(RefreshTokenName, token, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7),
        });
    }

    public string? GetAccessToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[AccessTokenName];
    }

    public string? GetRefreshToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[RefreshTokenName];
    }

    public void ClearAuthCookies()
    {
        var context = _httpContextAccessor.HttpContext!;
        context.Response.Cookies.Delete(AccessTokenName);
        context.Response.Cookies.Delete(RefreshTokenName);
    }
}