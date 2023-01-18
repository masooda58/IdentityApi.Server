using System;
using Microsoft.AspNetCore.Http;

namespace Jwt.Identity.Api.Server.Security
{
    public static class CookiesOptions
    {
        public static CookieOptions SetCookieOptions(DateTime expireDateTime)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                // Domain = client.BaseUrl.ToString(),
                SameSite = SameSiteMode.Lax,
                Expires = expireDateTime
            };
        }

        public static CookieOptions SetCookieOptionsPresist()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                // Domain = client.BaseUrl.ToString(),
                SameSite = SameSiteMode.Lax
            };
        }
    }
}