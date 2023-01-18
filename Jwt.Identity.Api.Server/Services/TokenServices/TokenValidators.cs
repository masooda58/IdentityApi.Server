using Common.Jwt.Authentication;
using Jwt.Identity.Domain.Token.Enum;
using Jwt.Identity.Domain.Token.ITokenServices;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EasyCaching.Core;
using Jwt.Identity.Domain.Shared.Models.CacheData;

namespace Jwt.Identity.Api.Server.Services.TokenServices
{
    public class TokenValidators : ITokenValidators
    {
        private readonly JwtSettingModel _jwtSetting;
        private readonly IEasyCachingProviderBase _cache;

        public TokenValidators(JwtSettingModel jwtSetting, IEasyCachingProviderBase cache)
        {
            _jwtSetting = jwtSetting;
            _cache = cache;
        }

        public bool Validate(string token, TokenType type)
        {
         
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            //in tanzimat dar clint ham hast

            string secret = type == TokenType.AccessToken ? _jwtSetting.Secret : _jwtSetting.RefreshSecret;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _jwtSetting.ValidAudience,
                ValidIssuer = _jwtSetting.ValidIssuer,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,


                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };
            try
            {
                jwtTokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ClaimsPrincipal GetClaimPrincipalValidatedToken(string token, TokenType type = TokenType.AccessToken)
        {
            var isServerCreatToken= _cache.Get<ValidTokenCacheModel>(token);
            if (!isServerCreatToken.HasValue)
            {
                return null;
            }
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                //in tanzimat dar clint ham hast
                
                string secret = type == TokenType.AccessToken ? _jwtSetting.Secret : _jwtSetting.RefreshSecret;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = _jwtSetting.ValidAudience,
                    ValidIssuer = _jwtSetting.ValidIssuer,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,


                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };

                var claimPrincipal = jwtTokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return claimPrincipal;
            }
            catch (Exception e)
            {
                return null;
            }


        }
    }
}