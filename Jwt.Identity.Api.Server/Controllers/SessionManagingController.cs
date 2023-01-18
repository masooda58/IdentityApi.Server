using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Resources;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.Shared;
using Jwt.Identity.Domain.User.Enum;
using Jwt.Identity.Framework.Response;

namespace Jwt.Identity.Api.Server.Controllers
{
    [Route("[ProjectRout]")]
    [ApiController]
    public class SessionManagingController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public SessionManagingController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> SetSessionId()
        {
            var baseRout= HttpContext.Request.Host.ToString();
            var client = await _unitOfWork.ClientRepository.GetByBaseUrlNotraking(baseRout);
            if (client == null)
            {
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            }
            var sessionId= Request.Cookies[KeyRes.SessionId]??Guid.NewGuid().ToString();
           
            if (client is {LoginType: LoginType.Cookie or LoginType.TokenAndCookie})
            {
                Response.Cookies.Append(KeyRes.SessionId, sessionId);
                return Ok(new ResultResponse(true, $"{KeyRes.SessionId}", sessionId));
            }
            if (client is {LoginType: LoginType.Token })
            {
                
                return Ok(new ResultResponse(true, $"{KeyRes.SessionId}", sessionId));
            }

            return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));
        }
    }
}
