using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.InitialData;
using Jwt.Identity.Framework.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Resources;

namespace Jwt.Identity.Api.Server.Controllers
{

    [Route("[ProjectRout]/")]
    [ApiController]

    public class InstalitionController : ControllerBase
    {
        private readonly IdentityContext _context;
        private readonly ILogger _logger;

        public InstalitionController(IdentityContext context, ILogger<InstalitionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("Instalation")]
        public async Task<ActionResult> Installation()
        {
            try
            {
                InitialDb.InstallationDb(_context);
                return Ok(new ResultResponse(true, "Db Configured"));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                if (!await _context.Database.CanConnectAsync())
                    return Ok(new ResultResponse(false, "Db Cant Connect"));
                return Ok(new ResultResponse(false,ErrorRes.NotComplete,e ));

            }
        }
    }
}
