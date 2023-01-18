using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Jwt.Identity.BoursYarServer.Helpers.Extensions;
using Jwt.Identity.BoursYarServer.Models.SettingModels;
using Jwt.Identity.BoursYarServer.Resources;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Framework.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jwt.Identity.BoursYarServer.Areas.Account.pages
{
    public class SendConfirmationCodeModel : PageModel
    {
        private string _normalEmailOrPhone;

        [BindProperty]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        // [EmailAddress(ErrorMessage = "{0} وارد شده صحیح نمی باشد")]
        [PhoneOrEmail]
        [Display(Name = "ایمیل یا شماره موبایل")]
        public string EmailOrPhone
        {
            get => _normalEmailOrPhone.ToNormalPhoneNo();
            set => _normalEmailOrPhone = value;
        }

        [BindProperty]
        [Display(Name = "کد تایید")]

        public string VerifySmsCode { get; set; }

        public string ReturnUrl { get; set; }


        public void OnGet(string returnUrl, string emailOrPhone = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            EmailOrPhone = emailOrPhone;

            if (!TempData.ContainsKey(TempDataDict.ShowEmailConfirmationMessage) &&
                !TempData.ContainsKey(TempDataDict.ShowTotpConfirmationCode))
                TempData[TempDataDict.ShowSendCofirmationCode] = true;
        }

        public async Task<ActionResult> OnPostConfirmationEmailOrPhoneAsync(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                #region Send email confirmation

                if (EmailOrPhone.Contains("@"))
                {
                    var userExist = await _userManager.FindByEmailAsync(EmailOrPhone);
                    if (userExist == null)
                    {
                        TempData[TempDataDict.ShowEmailConfirmationMessage] = true;
                        ModelState.AddModelError(string.Empty, $"ایمیل {EmailOrPhone} قبلا در سایت ثبت نام ننموده است");
                        //   return RedirectToPage("RegisterConfirmation", new { email = EmailOrPhone });
                        return Page();
                    }

                    //await SendEmailConfirmationAsync(EmailOrPhone);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(userExist);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/ConfirmEmail",
                        null,
                        new { area = "Account", email = EmailOrPhone, code },
                        Request.Scheme);

                    await _emailSender.SendEmailAsync(EmailOrPhone, "تاییدیه ایمیل",
                        $"جهت تایید ایمیل اینجا <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک نمایید</a>.",
                        true);
                    TempData[TempDataDict.ShowEmailConfirmationMessage] = true;
                    return RedirectToPage("/SendConfirmationCode", new { returnUrl, emailOrPhone = EmailOrPhone });


                    //  return RedirectToPage("RegisterConfirmation", new { EmailOrPhone });
                }

                #endregion

                #region send sms Confirmation

                else
                {
                    var userExist = await _userManager.Users
                        .AnyAsync(user => user.PhoneNumber == EmailOrPhone);

                    if (!userExist)
                    {
                        TempData[TempDataDict.ShowTotpConfirmationCode] = true;
                        ModelState.AddModelError(string.Empty,
                            $"شماره تلفن {EmailOrPhone} قبلا در سایت ثبت نام ننموده است");
                        return Page();
                    }

                    var resualtSendTotpCode =
                        await _totpCode.SendTotpCodeAsync(EmailOrPhone, TotpTypeCode.TotpAccountConfirmationCode);
                    if (resualtSendTotpCode.Successed)
                    {
                        TempData[TempDataDict.ShowTotpConfirmationCode] = true;
                        ReturnUrl = returnUrl;
                        return Page();
                    }


                    TempData[TempDataDict.Error_TotpCode] = resualtSendTotpCode.Message;
                    TempData[TempDataDict.ShowSendCofirmationCode] = true;
                    TempData[TempDataDict.ShowTotpConfirmationCode] = false;
                    ReturnUrl = returnUrl;
                    return Page();
                }

                #endregion
            }

            return Page();
        }

        public async Task<ActionResult> OnPostConfirmationSmsCode(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ReturnUrl = returnUrl;
            if (string.IsNullOrEmpty(VerifySmsCode))
            {
                ModelState.AddModelError(string.Empty, "کد را وارد نمایید");
                TempData[TempDataDict.ShowTotpConfirmationCode] = true;
                return RedirectToPage("/SendConfirmationCode", new { returnUrl, emailorPhone = EmailOrPhone });
            }

            var resualtConfirmTotpCodeAsync = await _totpCode
                .ConfirmTotpCodeAsync(EmailOrPhone, VerifySmsCode, TotpTypeCode.TotpAccountConfirmationCode);
            if (!resualtConfirmTotpCodeAsync.Successed &&
                resualtConfirmTotpCodeAsync.Message.FirstOrDefault() != ErrorMessageRes.WrongTotpInput)
            {
                TempData[TempDataDict.Error_TotpCode] = ErrorMessageRes.CodeExpire;

                return RedirectToPage("/SendConfirmationCode", new { returnUrl, emailorPhone = EmailOrPhone });
            }


            if (resualtConfirmTotpCodeAsync.Successed)
            {
                //phone number confirmation
                var user = await _userManager.FindByNameAsync(EmailOrPhone);
                if (user == null)
                {
                    TempData[TempDataDict.Error_TotpCode] = ErrorMessageRes.UnknownTotpError;
                    return RedirectToPage("/SendConfirmationCode", new { returnUrl, emailOrPhone = EmailOrPhone });
                }

                var tokenPhone = await _userManager.GenerateChangePhoneNumberTokenAsync(user, EmailOrPhone);
                var confirmMoleNumber = await _userManager.ChangePhoneNumberAsync(user, EmailOrPhone, tokenPhone);
                //signin user    
                await _signInManager.SignInAsync(user, false);
                return LocalRedirect(ReturnUrl);
            }


            // کد وارد شده صحیح نیست
            TempData[TempDataDict.ShowTotpConfirmationCode] = true;
            TempData[TempDataDict.Error_TotpCode] = resualtConfirmTotpCodeAsync.Message;


            return RedirectToPage("./SendConfirmationCode", new { returnUrl, emailOrPhone = EmailOrPhone });
        }

        #region CTOR

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IPhoneTotpProvider _totp;
        private readonly ISmsSender _smsSender;
        private readonly TotpSettings _options;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITotpCode _totpCode;

        public SendConfirmationCodeModel(UserManager<ApplicationUser> userManager, ILogger<RegisterModel> logger,
            IEmailSender emailSender, IPhoneTotpProvider totp, ISmsSender smsSender, IOptions<TotpSettings> options,
            SignInManager<ApplicationUser> signInManager, ITotpCode totpCode)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _totp = totp;
            _smsSender = smsSender;
            _signInManager = signInManager;
            _totpCode = totpCode;
            _options = options?.Value ?? new TotpSettings();
        }

        #endregion
    }
}