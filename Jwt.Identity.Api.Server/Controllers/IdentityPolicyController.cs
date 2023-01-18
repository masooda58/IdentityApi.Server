using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Resources;
using Jwt.Identity.Api.Server.Security;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.IdentityPolicy.Command;
using Jwt.Identity.Domain.IdentityPolicy.Entity;
using Jwt.Identity.Domain.IdentityPolicy.Query;
using Jwt.Identity.Framework.Response;
using Jwt.Identity.Framework.Tools;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jwt.Identity.Api.Server.Controllers
{
    [Route("[ProjectRout]/")]
    [ApiController]
    public class IdentityPolicyController : ControllerBase
    {
        private readonly ILogger _logger;
     private readonly IMediator _mediator;
    


        public IdentityPolicyController( IMediator mediator, ILogger<IdentityPolicyController> logger)
        {
            _mediator = mediator;
            _logger = logger;
            
    
        }

        [HttpGet("Get")]
        public async Task<ActionResult> Get()
        {

            try
            {
                var setting= await _mediator.Send(new GetIdentityPolicy());
                return Ok(new ResultResponse(true, "policy Setting", setting));
            }
            catch (Exception e)
            {
               _logger.LogError(e.Message);
               return BadRequest(new ResultResponse(false,ErrorRes.GetError));
            }
        }

        [HttpPut("Put")]
        public async Task<ActionResult> Put([FromBody] UpdatePolicy setting)
        {
            try
            {
                var chekValidation=  Utilitis.CheckModelValidation<IdentitySettingPolicy>(setting.IdentitySetting);
                if (chekValidation.Successed)
                {
                    return Ok(await _mediator.Send(setting));
                }

                return BadRequest(chekValidation);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new ResultResponse(false,ErrorRes.UppdateError));
            }
        }

  
    }
}
