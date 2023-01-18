using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Jwt.Identity.BoursYarServer.Helpers.Extensions;
using Jwt.Identity.BoursYarServer.Resources;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Framework.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Jwt.Identity.BoursYarServer.Areas.Account.pages
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterModel> _logger;
        private readonly  IDistributedCache _memoryCache;
        private readonly SignInManager<ApplicationUser> _signInManager;


        private readonly ITotpCode _totpCode;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,  IDistributedCache memoryCache, ITotpCode totpCode, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _memoryCache = memoryCache;
            _totpCode = totpCode;
            _emailSender = emailSender;
        }

        [BindProperty] public InputModel Input { get; set; }
        //[TempData]
        //public TotpTempData Ptc { get; set; }


        public string ReturnUrl { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                Redirect("/");
            }
            else
            {
                ReturnUrl = returnUrl ?? Url.Content("~/");

                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            #region Register email And mobileNo base on Visio flow

            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByNameAsync(Input.EmailOrPhone);

                #region Check user Exist

                if (userExist != null && Input.EmailOrPhone.Contains("@"))
                {
                    ModelState.AddModelError(string.Empty,
                        $"ایمیل {Input.EmailOrPhone} قبلا در سایت ثبت نام نموده است");
                    return Page();
                }

                if (userExist != null && !Input.EmailOrPhone.Contains("@"))
                {
                    ModelState.AddModelError(string.Empty,
                        $"شماره {Input.EmailOrPhone} قبلا در سایت ثبت نام نموده است");
                    return Page();
                }

                #endregion

                #region Create user

                var user = Input.EmailOrPhone.Contains("@")
                    ? new ApplicationUser { UserName = Input.EmailOrPhone, Email = Input.EmailOrPhone }
                    : new ApplicationUser { UserName = Input.EmailOrPhone, PhoneNumber = Input.EmailOrPhone };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "در ساختن کاربر مشکلی رخداده است.");
                    return Page();
                }

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    //var newUser =await _userManager.FindByNameAsync(Input.EmailOrPhone);

                    #region Send Email Confirm

                    if (Input.EmailOrPhone.Contains("@"))
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var callbackUrl = Url.Page(
                            "/ConfirmEmail",
                            null,
                            new { area = "Account", email = Input.EmailOrPhone, code },
                            Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.EmailOrPhone, "تاییدیه ایمیل",
                            $"جهت تایید ایمیل اینجا <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک نمایید</a>.",
                            true);
                        TempData[TempDataDict.ShowEmailConfirmationMessage] = true;
                        return RedirectToPage("/SendConfirmationCode",
                            new { returnUrl, emailOrPhone = Input.EmailOrPhone });
                    }

                    #endregion

                    #region send sms confirm

                    _memoryCache.Remove(TotpTypeDict.TotpAccountConfirmationCode);
                    var resualtSendTotpCodeAsync =
                        await _totpCode.SendTotpCodeAsync(Input.EmailOrPhone, TotpTypeCode.TotpAccountConfirmationCode);
                    if (resualtSendTotpCodeAsync.Successed)
                    {
                        TempData[TempDataDict.ShowTotpConfirmationCode] = true;
                        return RedirectToPage("/SendConfirmationCode",
                            new { returnUrl, emailOrPhone = Input.EmailOrPhone });
                    }

                    ModelState.AddModelError(string.Empty, ErrorMessageRes.UnkonwnError);
                    return Page();

                    #endregion
                }

                await _signInManager.SignInAsync(user, false);
                return LocalRedirect(returnUrl);

                #endregion
            }

            #endregion

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //remot page validation
        public async Task<JsonResult> OnPostCheckEmail()
        {
            var user = await _userManager.FindByEmailAsync(Input.EmailOrPhone);
            if (user == null)
            {
                //string normalMobileNo = "989" + Input.EmailOrPhone.Substring(Input.EmailOrPhone.Length - 9);
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == Input.EmailOrPhone);
                // var validPhone = user == null;
                return new JsonResult(user == null
                    ? true
                    : $"شماره تلفن {Input.EmailOrPhone} قبلا در سایت ثبت نام نموده است");
            }

            //var validEmail = user == null;
            return new JsonResult($"ایمیل {Input.EmailOrPhone} قبلا در سایت ثبت نام نموده است");
        }

        public class InputModel
        {
            private string _normalEmailOrPhone;

            [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
            // [EmailAddress(ErrorMessage = "{0} وارد شده صحیح نمی باشد")]
            [PhoneOrEmail]
            [Display(Name = "ایمیل یا شماره موبایل")]
            // ErrorMessage = "ایمیل یا شماره موبایل  قبلا  در سایت ثبت نام کرده است",
            [PageRemote(
                HttpMethod = "post",
                PageHandler = "CheckEmail",
                AdditionalFields = "__RequestVerificationToken"
            )]

            //[Remote(
            //    "checkemail",
            //    "WeatherForecast",
            //    ErrorMessage = "Email Address already exists",
            //    AdditionalFields = "__RequestVerificationToken",
            //    HttpMethod = "post"
            //)]

            public string EmailOrPhone
            {
                get => _normalEmailOrPhone.ToNormalPhoneNo();
                set => _normalEmailOrPhone = value;
            }

            [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
            [StringLength(100, ErrorMessage = "{0} حداقل {2} و حداکثر {1} باشد", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تکرار رمز عبور")]
            [Compare("Password", ErrorMessage = "{0} و {1} مطابقت ندارند")]
            public string ConfirmPassword { get; set; }
        }
    }
}