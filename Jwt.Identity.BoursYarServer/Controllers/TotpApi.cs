using System.Threading.Tasks;
using Jwt.Identity.BoursYarServer.Models.ViewModel;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Framework.Response;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.Identity.BoursYarServer.Controllers
{
    //[Route("totp/[controller]")]
    //[ApiController]
    public class TotpApi : ControllerBase
    {
        private readonly ITotpCode _totpService;

        public TotpApi(ITotpCode totpService)
        {
            _totpService = totpService;
        }

        [HttpPost]
        [Route("SendTotp")]
        public async Task<JsonResult> SendTotp([FromForm] TotpDto totpDto)
        {
            var result = await _totpService.SendTotpCodeAsync(totpDto.PhoneNumber, totpDto.TotpType);
            return new JsonResult(result);
        }

        [HttpPost]
        [Route("ConfirmTotp")]
        public async Task<ResultResponse> ConfirmTotp(string phoneNo, [FromForm] string code)
        {
            var result =
                await _totpService.ConfirmTotpCodeAsync(phoneNo, code, TotpTypeCode.TotpAccountPasswordResetCode);
            return result;
            //return new PartialViewResult()
            //{
            //    ViewName = "\\Areas\\Account\\pages\\Shared\\_TotpCofirmationPrtial.cshtml",
            //    ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            //    {
            //        Model = new TotpDto() {   
            //            TotpType = TotpTypeCode.TotpAccountPasswordResetCode,
            //            PhoneNumber = "989123406286"

            //        },


            //    }

            //};
        }
    }
}
//[HttpPost]
//[Route("ConfirmTotp")]
//public async Task<JsonResult> ConfirmTotp([FromForm]TotpDto totpDto,[FromForm]string code)
//{


//    var result = await _TotpService.ConfirmTotpCodeAsync(totpDto.PhoneNumber, code,totpDto.TotpType);
//    return new JsonResult(result);
//}