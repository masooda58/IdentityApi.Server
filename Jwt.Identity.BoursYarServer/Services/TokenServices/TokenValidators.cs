using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jwt.Identity.Domain.Token.Enum;
using Jwt.Identity.Domain.Token.ITokenServices;
using Microsoft.IdentityModel.Tokens;

namespace Jwt.Identity.BoursYarServer.Services.TokenServices
{
      public class TokenValidators : ITokenValidators
    {
        private readonly JwtSettingModel _jwtSetting;

        public TokenValidators(JwtSettingModel jwtSetting)
        {
            _jwtSetting = jwtSetting;
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