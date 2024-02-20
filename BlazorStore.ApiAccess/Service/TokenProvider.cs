

using BlazorStore.ApiAccess.Service.IService;
using BlazorStore.Common;
using BlazorStore.Dto;
using Microsoft.AspNetCore.Http;

namespace BlazorStore.ApiAccess.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _hca;

        public TokenProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }

        public void ClearToken()
        {
            _hca.HttpContext?.Response.Cookies.Delete(SD.JwtAccessTokenCookie);
            _hca.HttpContext?.Response.Cookies.Delete(SD.JwtRrefreshTokenCookie);
            _hca.HttpContext?.Response.Cookies.Delete(SD.ApiXsrfCookie);
        }

        public TokenDto? GetToken()
        {
            try
            {
                string? accessToken = null;
                string? refreshToken = null;
                string? xsrfToken = null;
                bool hasAccessToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.JwtAccessTokenCookie, out accessToken) ?? false;
                bool hasRefreshToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.JwtRrefreshTokenCookie, out refreshToken) ?? false;
                bool hasXsrfToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.ApiXsrfCookie, out xsrfToken) ?? false;
                return new TokenDto(AccessToken: accessToken, XsrfToken: xsrfToken, RefreshToken: refreshToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetToken(TokenDto tokenDto)
        {
            ClearToken();

            if (tokenDto.AccessToken is not null)
                _hca.HttpContext?.Response.Cookies.Append(
                    SD.JwtAccessTokenCookie,
                    tokenDto.AccessToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(SD.JwtAccessTokenExpiry),
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });

            if (tokenDto.XsrfToken is not null)
                _hca.HttpContext?.Response.Cookies.Append(
                    SD.ApiXsrfCookie,
                    tokenDto.XsrfToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(SD.JwtAccessTokenExpiry),
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });

            if (tokenDto.RefreshToken is not null)
                _hca.HttpContext?.Response.Cookies.Append(
                    SD.JwtRrefreshTokenCookie,
                    tokenDto.RefreshToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(SD.JwtRefreshTokenExpiry),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });
        }
    }
}