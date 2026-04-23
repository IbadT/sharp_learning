namespace MyTextApi.Interfaces;

public interface ICookieService
{
    void SetAccessToken(string token, DateTime expiry);
    void SetRefreshToken(string token);
    string? GetAccessToken();
    string? GetRefreshToken();
    void ClearAuthCookies();
}