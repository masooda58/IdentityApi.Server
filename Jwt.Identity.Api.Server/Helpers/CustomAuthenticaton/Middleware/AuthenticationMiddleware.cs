//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using Jwt.Identity.Data.UnitOfWork;
//using Microsoft.AspNetCore.Http;

//namespace Jwt.Identity.Api.Server.Helpers.CustomAuthenticaton.Middleware
//{
//    public class AuthenticationMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly UnitOfWork _unitOfWork;

//        // Dependency Injection
//        public AuthenticationMiddleware(RequestDelegate next, UnitOfWork unitOfWork)
//        {
//            _next = next;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task Invoke(HttpContext context)
//        { 
//                var reqHost = context.Request.Host.ToString();
//                var client = await _unitOfWork.ClientRepository.GetByBaseUrlNotraking(reqHost);
//                switch (client)
//                {
//                    case null:
//                        await _next(context);
                   
//                    case { LoginType: LoginType.Token  }:
//                    {
                       
//                       context.User = new ClaimsPrincipal(await BearerToken());

//                    }
//                    case { LoginType: LoginType.Cookie  }:
//                    {
                        
//                        context.User = new ClaimsPrincipal(await CookieToken());
//                    }
//                    case { LoginType: LoginType.TokenAndCookie }:
//                    {
//                        var cookieAuthentication = await CookieToken();
//                        return cookieAuthentication.Succeeded ? cookieAuthentication : await BearerToken();
//                    }
//                    default:
//                        await _next(context);
//                }
           
          
 
//                context.User = new ClaimsPrincipal(identity);
            
//            await _next(context);
//        }
//    }
//}
