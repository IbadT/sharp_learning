using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MyTextApi.Services;

public class CookieJwtBearerHandler : JwtBearerHandler
{
    public CookieJwtBearerHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder
    )
    { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Сначала пробуем стандартный Bearer из заголовка
        var result = await base.HandleAuthenticateAsync();
        if (result.Succeeded) return result;

        // Если не удалось - пробуем из cookie
        if (Context.Request.Cookies.TryGetValue("access_token", out var token))
        {
            Context.Request.Headers.Append("Authorization", $"Bearer {token}");
            return await base.HandleAuthenticateAsync();
        }

        return AuthenticateResult.NoResult();
    }

}