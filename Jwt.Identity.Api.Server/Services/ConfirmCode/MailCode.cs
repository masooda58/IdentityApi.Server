using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Jwt.Identity.Api.Server.Resources;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Domain.IServices.Email;
using Jwt.Identity.Domain.IServices.Email.Enum;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Framework.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Jwt.Identity.Api.Server.Services.ConfirmCode
{
    public class MailCode : IMailCode
    {
        private readonly IEmailSender _emailSender;

        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MailCode(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            IHttpContextAccessor context)
        {
            _userManager = userManager;
            _emailSender = emailSender;

            _httpContext = context;
        }

        public async Task<ResultResponse> SendMailCodeAsync(ApplicationUser user, MailTypeCode type,
            string callbackUri = null)
        {
            if (callbackUri == null) return new ResultResponse(false, MessageRes.CallBackUrlNotValid);
            switch (type)
            {
                case MailTypeCode.MailAccountConfirmationCode:
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


                    var callbackUrl = callbackUri + $"?email={user.Email}&code={code}";


                    await _emailSender.SendEmailAsync(user.Email, "تاییدیه ایمیل",
                        $"جهت تایید ایمیل اینجا <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک نمایید</a>.",
                        true);

                    return new ResultResponse(true, MessageRes.EmailSent);
                }
                case MailTypeCode.MailAccountPasswordResetCode:
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


                    var callbackUrl = callbackUri + $"?email={user.Email}&code={code}";


                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Reset Password",
                        $"جهت ریست پسورد خود اینجا<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک نمایید</a>.",
                        true);

                    return new ResultResponse(true, MessageRes.EmailSent);
                }
                default:
                    return new ResultResponse(false, MessageRes.UnkonwnError);
            }
        }
    }
}