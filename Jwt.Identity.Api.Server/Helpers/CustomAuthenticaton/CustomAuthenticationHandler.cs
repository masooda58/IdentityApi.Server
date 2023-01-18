using EasyCaching.Core;
using EasyCaching.HybridCache;
using Jwt.Identity.Api.Server.Resources;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.Shared;
using Jwt.Identity.Domain.Token.ITokenServices;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Domain.User.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Jwt.Identity.Domain.Shared.Models.CacheData;

namespace Jwt.Identity.Api.Server.Helpers.CustomAuthenticaton
{

    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAutenthicationOptions>
    {
        private readonly ITokenValidators _tokenValidators;
        private readonly UnitOfWork _unitOfWork;
        //private readonly IEasyCachingProviderFactory _cacheFactory;
       // private readonly IEasyCachingProviderBase _cache;


        public CustomAuthenticationHandler(IOptionsMonitor<CustomAutenthicationOptions> options,
              ILoggerFactory logger,
              UrlEncoder encoder,
              ISystemClock clock,
              ITokenValidators tokenValidators,
              UnitOfWork unitOfWork
             ) : base(options, logger, encoder, clock)
        {
            _tokenValidators = tokenValidators;
            _unitOfWork = unitOfWork;
            
           
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var reqHost = Request.Host.ToString();
                var client = await _unitOfWork.ClientRepository.GetByBaseUrlNotraking(reqHost);
                switch (client)
                {
                    case null:
                        return AuthenticateResult.Fail(MessageRes.ClientNoExist);
                    case { LoginType: LoginType.Token }:
                        return await ExtractBearerTokenAsync();
                    case { LoginType: LoginType.Cookie }:
                        return await ExtractCookieTokenAsync();
                    case { LoginType: LoginType.TokenAndCookie }:
                        {
                            var cookieAuthentication = await ExtractCookieTokenAsync();
                            return cookieAuthentication.Succeeded ? cookieAuthentication : await ExtractBearerTokenAsync();
                        }
                    default:
                        return AuthenticateResult.Fail(MessageRes.Unauthorize);
                }
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail(MessageRes.Unauthorize);
            }
        }




        private async Task<AuthenticateResult> ExtractCookieTokenAsync()
        {
            var isCookieToken = Request.Cookies.TryGetValue(KeyRes.Access_TokenSession, out var cookieToken);
            if (isCookieToken && !string.IsNullOrEmpty(cookieToken))
            {
                return await CheckTokenValidationAsync(cookieToken);
            }

            return AuthenticateResult.Fail(MessageRes.Unauthorize);
        }

        private async Task<AuthenticateResult> CheckTokenValidationAsync(string token)
        {
            var claimPrincipal = _tokenValidators.GetClaimPrincipalValidatedToken(token);

            if (claimPrincipal == null)
            {
                return AuthenticateResult.Fail(MessageRes.Unauthorize);
            }
            return AuthenticateTokenResult(claimPrincipal);
           // var isServerCreatToken=await _cache.GetAsync<ValidTokenCacheModel>(token);
           //return !isServerCreatToken.HasValue ?
           //    AuthenticateResult.Fail(MessageRes.Unauthorize)
           //    : AuthenticateTokenResult(claimPrincipal);

           //var userId = claimPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            //var isHybridCache = _cache is HybridCachingProvider;
            //var cache = isHybridCache ? _cacheFactory.GetCachingProvider("r1") : _cacheFactory.GetCachingProvider("m1");

            //var currentUserLogin = await cache.GetByPrefixAsync<CurrentUserLogin>(userId);
            //if (currentUserLogin.Count == 0)
            //{
            //    return AuthenticateResult.Fail(MessageRes.Unauthorize);
            //}

            //foreach (var user in currentUserLogin)
            //{
            //    if (user.Value.Value.AccessToken == token)
            //    {
            //        return AuthenticateTokenResult(claimPrincipal);
            //    } 
            //}





           
        }

        private AuthenticateResult AuthenticateTokenResult(ClaimsPrincipal claimPrincipal)
        {



            var ticket = new AuthenticationTicket(claimPrincipal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private async Task<AuthenticateResult> ExtractBearerTokenAsync()
        {
            string authorization = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.Fail(MessageRes.Unauthorize);
            }

            var isBearerToken = authorization.StartsWith("bearer", StringComparison.OrdinalIgnoreCase);
            if (!isBearerToken)
            {
                return AuthenticateResult.Fail(MessageRes.Unauthorize);
            }

            var token = authorization.Substring("Bearer".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail(MessageRes.Unauthorize);
            }

            return await CheckTokenValidationAsync(token);
            // var isTokenValid = _tokenValidators.Validate(token);

        }
    }


}
