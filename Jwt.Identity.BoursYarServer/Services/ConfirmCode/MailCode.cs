using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Domain.IServices.Email;
using Jwt.Identity.Domain.IServices.Email.Enum;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Framework.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Jwt.Identity.BoursYarServer.Services.ConfirmCode
{
    public class MailCode : IMailCode
    {
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUrlHelper _url;
        private readonly UserManager<ApplicationUser> _userManager;

        public MailCode(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IUrlHelper url,
            IHttpContextAccessor context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _url = url;
            _httpContext = context;
        }

        public async Task<ResultResponse> SendMailCodeAsync(ApplicationUser user, MailTypeCode type,
            string callbackUri = null)
        {
            switch (type)
            {
                case MailTypeCode.MailAccountConfirmationCode:
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


                    var callbackUrl = _url.Page(
                        "/ConfirmEmail",
                        null,
                        new { area = "Account", email = user.Email, code },
                        _httpContext.HttpContext?.Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email, "تاییدیه ایمیل",
                        $"جهت تایید ایمیل اینجا <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک نمایید</a>.",
                        true);

                    return new ResultResponse(true, "");
                }
                case MailTypeCode.MailAccountPasswordResetCode:
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


                    var callbackUrl = _url.Page(
                        "/ResetPassword",
                        null,
                        new { area = "Account", userEmailOrPhone = user.Email, code },
                        _httpContext.HttpContext?.Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Reset Password",
                        $"جهت ریست پسورد خود اینجا<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک نمایید</a>.",
                        true);

                    return new ResultResponse(true, "");
                }
                default:
                    return new ResultResponse(false, "خطای نوع ارسال");
            }
        }
    }
}