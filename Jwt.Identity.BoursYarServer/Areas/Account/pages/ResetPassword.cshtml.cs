using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.BoursYarServer.Resources;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Domain.User.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Jwt.Identity.BoursYarServer.Areas.Account.pages
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly IPhoneTotpProvider _totp;
        private readonly ITotpCode _totpCode;
        private readonly UserManager<ApplicationUser> _userManager;


        public ResetPasswordModel(UserManager<ApplicationUser> userManager, IPhoneTotpProvider totp, ITotpCode totpCode)
        {
            _userManager = userManager;
            _totp = totp;
            _totpCode = totpCode;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IActionResult OnGet(string code = null, string userEmailOrPhone = null)
        {
            // 
            if (!TempData.ContainsKey(TempDataDict.ShowTotpResetCode))
            {
                if (code == null || userEmailOrPhone == null) return BadRequest("کد تایید  اشتباه است");


                Input = new InputModel
                {
                    EmailOrPhone = userEmailOrPhone,
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }

            if (userEmailOrPhone == null) return BadRequest("کد تایید  اشتباه است");

            //اگر پارمتر ورودی وجود داشته باشد و کاربر آن را دستی نزده باشد
            Input = new InputModel
            {
                EmailOrPhone = userEmailOrPhone,
                Code = ""
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            #region reset password by Email

            var user = await _userManager.FindByNameAsync(Input.EmailOrPhone);
            if (user == null && Input.EmailOrPhone.Contains("@"))
                // Don't reveal that the user does not exist

                return RedirectToPage("./Login");

            if (Input.EmailOrPhone.Contains("@"))
            {
                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

                if (result.Succeeded) return RedirectToPage("./Login");

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            #endregion


            #region reset password by Totp

            var resulConfirmationResetTotpCode =
                await _totpCode.ConfirmTotpCodeAsync(Input.EmailOrPhone, Input.Code,
                    TotpTypeCode.TotpAccountPasswordResetCode);
            // اگر کد ارسالی منقضی شده باشد
            if (!resulConfirmationResetTotpCode.Successed
                && resulConfirmationResetTotpCode.Message.FirstOrDefault() == ErrorMessageRes.CodeExpire)
            {
                TempData[TempDataDict.Error_TotpCode] = ErrorMessageRes.CodeExpire;
                return RedirectToPage("./ForgotPassword");
            }

            // کد تایید شود
            if (resulConfirmationResetTotpCode.Successed)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, Input.Password);
                if (result.Succeeded) return RedirectToPage("./Login");

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            // کد اشتباه وارد شود

            ModelState.AddModelError(string.Empty, resulConfirmationResetTotpCode.Message.FirstOrDefault());
            TempData[TempDataDict.ShowTotpResetCode] = true;
            return Page();

            #endregion
        }

        public async Task<JsonResult> OnPostCheckCode()
        {
            var phoneNo = TempData.Peek("PhoneNo")!.ToString();
            if (phoneNo != null)
            {
                var result =
                    await _totpCode.ConfirmTotpCodeAsync(phoneNo, Input.Code,
                        TotpTypeCode.TotpAccountPasswordResetCode);
                if (result.Successed) return new JsonResult("کد وارد شده صحیح است");

                return new JsonResult(result.Message);
            }

            if (phoneNo == null) return new JsonResult("کد تایید را وارد نمایید");

            return new JsonResult("کد وارد شده صحیح نمی باشد");
        }

        public class InputModel
        {
            public string EmailOrPhone { get; set; }

            [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
            [StringLength(100, ErrorMessage = "{0} حداقل {2} و حداکثر {1} باشد", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تکرار رمز عبور")]
            [Compare("Password", ErrorMessage = "{0} و {1} مطابقت ندارند")]
            public string ConfirmPassword { get; set; }


            [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
            [Display(Name = "کد تایید")]
            [PageRemote(
                HttpMethod = "post",
                PageHandler = "CheckCode",
                AdditionalFields = "__RequestVerificationToken"
            )]
            public string Code { get; set; }
        }

        ////private async Task<PhoneTotpResult> ConfirmResetTotpCod()
        ////{
        ////    // اگر سیستم کدی را ارسال نکرده باشد

        ////    #region absoulte Way

        ////    if (!TempData.ContainsKey(TempDataDict.TotpResetCode))
        ////    {
        ////        TempData[TempDataDict.Error_TotpCode] = "کد ارسالی منقضی شده است لطفا کد جدید دریافت کنید";
        ////        return new PhoneTotpResult(false, "کد ارسالی منقضی شده است لطفا کد جدید دریافت کنید");

        ////    }
        ////    // اگر کئ ارسالی منقضی شده باشد
        ////    var resetCodeTemp = TempData.Get<TotpTempData>(TempDataDict.TotpResetCode);
        ////    if (resetCodeTemp.ExpirationTime <= DateTime.Now)
        ////    {
        ////        TempData[TempDataDict.Error_TotpCode] = "کد ارسالی منقضی شده است لطفا کد جدید دریافت کنید";
        ////        return new PhoneTotpResult(false, "کد ارسالی منقضی شده است لطفا کد جدید دریافت کنید");

        ////    }


        ////    var mathResult = _totp.VerifyTotp(resetCodeTemp.SecretKey, Input.Code);
        ////    //اگر کد درست باشد
        ////    if (mathResult.Successed)
        ////    {
        ////        //phone number confirmation
        ////        var user = await _userManager.FindByNameAsync(resetCodeTemp.UserMobileNo);
        ////        if (user == null)
        ////        {
        ////            TempData[TempDataDict.Error_TotpCode] = "مشکلی پیش آمده مجدد درخواست کد نمایید";
        ////            return new PhoneTotpResult(false, "مشکلی پیش آمده مجدد درخواست کد نمایید");

        ////        }

        ////        return new PhoneTotpResult(true, "");

        ////    }

        ////    // اگر کد غلط باشد
        ////    TempData.Keep(TempDataDict.TotpResetCode);
        ////    TempData[TempDataDict.ShowTotpResetCode] = true;
        ////    //TempData[TempDataDict.Error_TotpCode] = "کد وارد شده صحیح نمی باشد";


        ////    return new PhoneTotpResult(false, "کد وارد شده صحیح نمی باشد");

        ////    //#endregion


        ////}
    }
}