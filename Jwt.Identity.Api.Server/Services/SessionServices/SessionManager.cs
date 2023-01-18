using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Resources;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.Shared;
using Jwt.Identity.Domain.User.Enum;
using Jwt.Identity.Framework.Response;
using Microsoft.AspNetCore.Http;

namespace Jwt.Identity.Api.Server.Services.SessionServices
{
    public class SessionManager
    {
        private readonly UnitOfWork _unitOfWork;

        public SessionManager(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultResponse> TrySetSessionId(HttpContext contex)
        {
            var baseRout= contex.Request.Host.ToString();
            var client = await _unitOfWork.ClientRepository.GetByBaseUrlNotraking(baseRout);
            if (client == null)
            {
                return new ResultResponse(false, MessageRes.ClientNoExist);
            }

            var sessionId = contex.Request.Cookies[KeyRes.SessionId];
            if (!string.IsNullOrEmpty(sessionId))
            {
                return new ResultResponse(true, $" SessionId in Cookie  {KeyRes.SessionId}", sessionId);
            }


            if ( sessionId==null && client is {LoginType: LoginType.Cookie or LoginType.TokenAndCookie})
            {
                sessionId = Guid.NewGuid().ToString();
                contex.Response.Cookies.Append(KeyRes.SessionId, sessionId);
                return new ResultResponse(true, $" SessionId set to Cookie   {KeyRes.SessionId}", sessionId);
            }

            sessionId = contex.Request.Headers[KeyRes.SessionId];
            if (!string.IsNullOrEmpty(sessionId))
            {
                return new ResultResponse(true, $" SessionId in Header  {KeyRes.SessionId}", sessionId);
            }

            if (string.IsNullOrEmpty(sessionId) && client is {LoginType: LoginType.Token })
            {
                
                return new ResultResponse(true, $"{KeyRes.SessionId}", sessionId);
            }

            return new ResultResponse(false, MessageRes.UnkonwnError);
        }
       
    }
}
