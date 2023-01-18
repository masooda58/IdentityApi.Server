using Jwt.Identity.Domain.Clients.Command;
using Jwt.Identity.Domain.Clients.Query;
using Jwt.Identity.Framework.Response;
using Jwt.Identity.Framework.Tools.PersianErrorHandelSqlException;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Resources;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jwt.Identity.Api.Server.Controllers
{
     [Route("[ProjectRout]/")]
     [ApiController]
    

    public class ClientController : ControllerBase
    {
        
        private readonly IWebHostEnvironment _env;
        private readonly IMediator _mediator;
        private readonly ILogger _loger;
        public ClientController(IMediator mediator, IWebHostEnvironment env, ILogger<ClientController> loger)
        {
            _mediator = mediator;
            _env = env;
            _loger = loger;
        }
        // GET: api/<ClientController>
        [HttpGet("GetAll")]
        
        public async Task<ActionResult> GetAll()
        {
           
            try
            {
                var result = await _mediator.Send(new GetAllClient());
                return Ok(new ResultResponse(true, "دریافت با موفقیت انجام شد", result));

            }
            catch (Exception e)
            {
                if (_env.IsDevelopment())
                {
                    throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
                }
                _loger.LogError("Error",e);
                return BadRequest(new ResultResponse(true, ErrorRes.GetError, e));
            }



        }

        // GET api/<ClientController>/5
        //[HttpPost("GetByClientId")]
        //public async Task<Client> GetByClientId(int clientId)
        //{

        //    return Ok("await _clientService.FindByConditionAsync(c => c.ClientId == clientId);");
        //}
        [HttpPost("GetByClientName")]
        public async Task<ActionResult> GetByClientName(string clientName)
        {
            try
            {
                var client = await _mediator.Send(new GetClient() { ClientName = clientName });
                return Ok(new ResultResponse(true,client.ClientName,client));
            }


            catch (Exception e)
            {
                if (_env.IsDevelopment())
                {
                    throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);

                }
                _loger.LogError("Error",e);
                return BadRequest(new ResultResponse(true,ErrorRes.GetError,clientName));
            }
        }


        // PUT api/<ClientController>/5
            //[HttpPut("Add")]
            //public async Task<ActionResult> AddClient([FromBody] UpSert client)
            //{
            //    try
            //    {
            //       await _mediator.Send(client);
            //         //await _unitOfWork.ClientRepository.InsertAsync(client);
            //         //_unitOfWork.Save();
            //        return Ok(client);
            //    }
            //    catch (Exception e)
            //    {
            //        throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
            //    }

            //}
            [HttpPut("Upsert")]
            public async Task<ActionResult> UpSertClient([FromBody] UpSert client)
            {


                try
                {
                    var result = await _mediator.Send(client);

                    return Ok(result);
                }
                catch (Exception e)
                {
                    if (_env.IsDevelopment())
                    {
                        throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
                    }
                    _loger.LogError("Error",e);
                    return BadRequest(new ResultResponse(false, ErrorRes.NotComplete, client.Client));
                }
            }

            // DELETE api/<ClientController>/5
            [HttpDelete("DeleteClient")]
            public async Task<ActionResult> DeleteClient(string clientName)
            {
                try
                {
                    await _mediator.Send(new DeletClient() { ClientName = clientName });
                    return Ok(new ResultResponse(true, $"کلاینت {clientName} حذف گردید",clientName));
                }
                catch (Exception e)
                {
                    if (_env.IsDevelopment())
                    {
                        throw ExceptionMessage.GetPerisanSqlExceptionMessage(e);
                    }
                    _loger.LogError("Error",e);
                    return BadRequest(new ResultResponse(false, ErrorRes.NotComplete, clientName));

                }



            }
        }
    }

