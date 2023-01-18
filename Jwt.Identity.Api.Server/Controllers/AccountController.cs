using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using EasyCaching.HybridCache;
using EasyCaching.InMemory;
using Jwt.Identity.Api.Server.Resources;
using Jwt.Identity.Api.Server.Security;
using Jwt.Identity.Data.IntialData;
using Jwt.Identity.Data.Repositories.UserRepositories;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.IServices.Email;
using Jwt.Identity.Domain.IServices.Email.Enum;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Domain.IServices.Totp.Request;
using Jwt.Identity.Domain.IServices.Totp.SettingModels;
using Jwt.Identity.Domain.Shared.Models.CacheData;
using Jwt.Identity.Domain.Token.Data;
using Jwt.Identity.Domain.Token.ITokenServices;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Domain.User.Enum;
using Jwt.Identity.Domain.User.RequestDto;
using Jwt.Identity.Framework.Extensions;
using Jwt.Identity.Framework.Response;
using Jwt.Identity.Framework.Tools;
using Jwt.Identity.Framework.Tools.PersianErrorHandelSqlException;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using static Google.Apis.Auth.GoogleJsonWebSignature;


namespace Jwt.Identity.Api.Server.Controllers
{
    [Route("[ProjectRout]/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /// <summary>
        ///     ثبت نام
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientName"></param>
        /// <param name="registerModel"></param>
        /// <returns> شماره موبایل یا ایمیل اگر عملیات موفقیت آمیز باشد  وگرنه خطا</returns>
        [HttpPost]
        [Route("register/{clientName}")]
        public async Task<IActionResult> Register(string clientName, [FromBody] RegisterRequest registerModel)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            #endregion

            #region Register email And mobileNo

            var userExist = registerModel.EmailOrPhone.Contains("@")
                ? await _userManager.FindByEmailAsync(registerModel.EmailOrPhone)
                : await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == registerModel.EmailOrPhone);

            #region Check user Exist

            if (userExist != null && registerModel.EmailOrPhone.Contains("@"))
            {
                ModelState.AddModelError(string.Empty,
                    $"ایمیل {registerModel.EmailOrPhone} قبلا در سایت ثبت نام نموده است");
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Conflict(new ResultResponse(false, errorList));
            }

            if (userExist != null && !registerModel.EmailOrPhone.Contains("@"))
            {
                ModelState.AddModelError(string.Empty,
                    $"شماره {registerModel.EmailOrPhone} قبلا در سایت ثبت نام نموده است");
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Conflict(new ResultResponse(false, errorList));
            }

            #endregion

            #region Create user

            var user = registerModel.EmailOrPhone.Contains("@")
                ? new ApplicationUser { UserName = registerModel.EmailOrPhone, Email = registerModel.EmailOrPhone }
                : new ApplicationUser
                    { UserName = registerModel.EmailOrPhone, PhoneNumber = registerModel.EmailOrPhone };
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            #endregion

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "در ساختن کاربر مشکلی رخداده است.");
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ResultResponse(false, errorList));
            }

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                //var newUser =await _userManager.FindByNameAsync(registerModel.EmailOrPhone);

                #region Send Email Confirm

                if (registerModel.EmailOrPhone.Contains("@"))
                {
                    var res = await _mailCode.SendMailCodeAsync(user,
                        MailTypeCode.MailAccountConfirmationCode, client.EmailConfirmPage);
                    ;
                    var successed = res.Successed;
                    var message = res.Message;
                    if (successed)
                        return Ok(new ResultResponse(true, MessageRes.EmailSent, registerModel.EmailOrPhone));

                    await _userManager.DeleteAsync(user);
                    return BadRequest(new ResultResponse(false, message));
                }

                #endregion

                #region send sms confirm

                // check ip not block
                var remoteIpAddress = HttpContext!.Connection.RemoteIpAddress;

                var isIpBlock = _memoryCache.Get<TempIpBlock>(remoteIpAddress!.ToString());
                if (isIpBlock.HasValue)
                    await _memoryCache.RemoveAsync(remoteIpAddress!.ToString());
                //......
                var resualtSendTotpCodeAsync =
                    await _totpCode.SendTotpCodeAsync(registerModel.EmailOrPhone,
                        TotpTypeCode.TotpAccountConfirmationCode);
                if (resualtSendTotpCodeAsync.Successed)
                {
                    #region Set Cooki for phoneNo

                    HttpContext.Response.Cookies.Append("TempSession",
                        _protector.Protect(registerModel.EmailOrPhone + TotpTypeCode.TotpAccountConfirmationCode),
                        CookiesOptions.SetCookieOptions(DateTime.Now.AddSeconds(2 * _totpSettings.Step)));

                    #endregion

                    return Ok(new ResultResponse(true, MessageRes.TotpSent,
                        registerModel.EmailOrPhone + TotpTypeCode.TotpAccountConfirmationCode));
                }

                ModelState.AddModelError(string.Empty, MessageRes.UnkonwnError);
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                await _userManager.DeleteAsync(user);
                return BadRequest(new ResultResponse(false, resualtSendTotpCodeAsync.Message));

                #endregion
            }

            // اگر کاربر نیاز به کانفرم اکانت نداشت
            var resultSignIn = await _signInManager.PasswordSignInAsync(registerModel.EmailOrPhone,
                registerModel.Password,
                false, true);

            if (resultSignIn.Succeeded)
            {
                var loginJwt = await LoginJwt(user, client.LoginType);
               
                return loginJwt ;
            }

            

            return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));

            #endregion
        }

        [HttpPost("login/{clientName}")]
        public async Task<IActionResult> Login(string clientName, [FromBody] LoginRequest loginModel)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            #endregion

            //  await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true

            var user = loginModel.EmailOrPhone.Contains("@")
                ? await _userManager.FindByEmailAsync(loginModel.EmailOrPhone)
                : await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == loginModel.EmailOrPhone);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, " نام کاربری یا رمز عبور اشتباه است. ");

                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return NotFound(new ResultResponse(false, errorList));
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginModel.Password,
                false, true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return await LoginJwt(user, client.LoginType);
            }

            // check locked
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return BadRequest(new ResultResponse(false, MessageRes.AccountLucked));
            }

            //check confirm
            if (user.PhoneNumberConfirmed || user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, MessageRes.WrongUserOrPass);
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }
            else
            {
                ModelState.AddModelError(string.Empty, MessageRes.AccountNotConFirm);
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }


            // If we got this far, something failed, redisplay form
        }

        [HttpPost("ExternalLogin/{clientName}")]
        public async Task<ActionResult> ExternalLogin(string clientName, string token)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            #endregion

            var payload = await ValidateAsync(token, new ValidationSettings
            {
                Audience = new[]
                {
                    "346095678950-dhuqj3ko64i5i1becqteg2v3rv9l8j6a.apps.googleusercontent.com"
                }
            });
            if (payload == null) return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                var newUser = new ApplicationUser { UserName = payload.Email, Email = payload.Email };
                var createdResult = await _userManager.CreateAsync(newUser);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                await _userManager.ConfirmEmailAsync(newUser, code);
                await _signInManager.SignInAsync(newUser, false, payload.Issuer);
                return await LoginJwt(newUser, client.LoginType);
            }

            await _signInManager.SignInAsync(user, false, payload.Issuer);
            return await LoginJwt(user, client.LoginType);
        }

        [HttpPost("ResetPassword/{clientName}")]
        public async Task<ActionResult> ResetPassword(string clientName, [FromBody] ForgetOrConfirmRequest input)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            #endregion


            var userExist = input.EmailOrPhone.Contains("@")
                ? await _userManager.FindByEmailAsync(input.EmailOrPhone)
                : await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == input.EmailOrPhone);
            if (userExist == null)
                // مشخص نمی کنیم که یوزر در سایت نمی باشد
                //ModelState.AddModelError(string.Empty, $"ایمیل {input.EmailOrPhone} قبلا در سایت ثبت نام ننموده است");

                return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));

            #region Send email reset

            if (input.EmailOrPhone.Contains("@"))
            {
                var res = await _mailCode.SendMailCodeAsync(userExist,
                    MailTypeCode.MailAccountPasswordResetCode, client.EmailResetPage);
                var successed = res.Successed;
                var message = res.Message;
                if (successed) return Ok(new ResultResponse(true, input.EmailOrPhone));

                return BadRequest(new ResultResponse(false, message));
            }

            #endregion

            #region Send sms rest

            var remoteIpAddress = HttpContext!.Connection.RemoteIpAddress;


            var resualtSendTotpCodeAsync =
                await _totpCode.SendTotpCodeAsync(input.EmailOrPhone,
                    TotpTypeCode.TotpAccountPasswordResetCode);
            switch (resualtSendTotpCodeAsync.Successed)
            {
                case true:

                    #region Set Cooki for phoneNo

                    HttpContext.Response.Cookies.Append("TempSession",
                        _protector.Protect(input.EmailOrPhone + TotpTypeCode.TotpAccountPasswordResetCode),
                        CookiesOptions.SetCookieOptions(DateTime.Now.AddSeconds(2 * _totpSettings.Step)));

                    #endregion

                    return Ok(new ResultResponse(true, MessageRes.TotpSent,
                        input.EmailOrPhone + TotpTypeCode.TotpAccountPasswordResetCode));
                default:
                    return BadRequest(new ResultResponse(false, resualtSendTotpCodeAsync.Message));

                #endregion
            }
        }

        [HttpPost("SendConfirmation/{clientName}")]
        public async Task<ActionResult> SendConfirmationAccount(string clientName,
            [FromBody] ForgetOrConfirmRequest input)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            #endregion


            var userExist = input.EmailOrPhone.Contains("@")
                ? await _userManager.FindByEmailAsync(input.EmailOrPhone)
                : await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == input.EmailOrPhone);
            if (userExist == null)
                // مشخص نمی کنیم که یوزر در سایت نمی باشد
                //ModelState.AddModelError(string.Empty, $"ایمیل {input.EmailOrPhone} قبلا در سایت ثبت نام ننموده است");

                return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));

            #region Send email reset

            if (input.EmailOrPhone.Contains("@"))
            {
                var res = await _mailCode.SendMailCodeAsync(userExist,
                    MailTypeCode.MailAccountConfirmationCode, client.EmailConfirmPage);
                var successed = res.Successed;
                var message = res.Message;
                if (successed) return Ok(new ResultResponse(true, input.EmailOrPhone));

                return BadRequest(new ResultResponse(false, message));
            }

            #endregion

            #region Send sms rest

            var remoteIpAddress = HttpContext!.Connection.RemoteIpAddress;


            var resualtSendTotpCodeAsync =
                await _totpCode.SendTotpCodeAsync(input.EmailOrPhone,
                    TotpTypeCode.TotpAccountConfirmationCode);
            switch (resualtSendTotpCodeAsync.Successed)
            {
                case true:

                    #region Set Cooki for phoneNo

                    HttpContext.Response.Cookies.Append("TempSession",
                        _protector.Protect(input.EmailOrPhone + TotpTypeCode.TotpAccountConfirmationCode),
                        CookiesOptions.SetCookieOptions(DateTime.Now.AddSeconds(2 * _totpSettings.Step)));

                    #endregion

                    return Ok(new ResultResponse(true, MessageRes.TotpSent,
                        input.EmailOrPhone + TotpTypeCode.TotpAccountConfirmationCode));
                default:
                    return BadRequest(new ResultResponse(false, resualtSendTotpCodeAsync.Message));
            }

            #endregion
        }

        [HttpPost("ConfirmTotpCookie/{clientName}")]
        public async Task<ActionResult> ConfirmTotp(string clientName, [FromBody] string code)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));

            #endregion

            var cookie = Request.Cookies.TryGetValue("TempSession", out var protectedPhoneAndType);
            if (!cookie) return BadRequest(new ResultResponse(false, MessageRes.CodeExpire));
            Debug.Assert(protectedPhoneAndType != null, nameof(protectedPhoneAndType) + " != null");
            var unProtectedPhoneAndType = _protector.Unprotect(protectedPhoneAndType);
            var phoneNo = unProtectedPhoneAndType.Substring(0, 12);
            var typeTotp = unProtectedPhoneAndType.Substring(12);
            var isCovertToTotpTyp = Enum.TryParse(typeTotp, out TotpTypeCode totpType);
            if (!isCovertToTotpTyp) return BadRequest(new ResultResponse(false, MessageRes.CodeExpire));

            var ResultResponse = await _totpCode.ConfirmTotpCodeAsync(phoneNo, code, totpType);
            if (ResultResponse.Successed)
            {
                var tempConfirmTotp = new TempConfirmTotp(phoneNo, totpType);
                //4 min
                _memoryCache.Set(phoneNo + typeTotp + "MobileConfirmed", tempConfirmTotp, TimeSpan.FromMinutes(4));

                #region remove coocki

                Response.Cookies.Delete("TempSession", new CookieOptions
                {
                    Secure = true
                });

                #endregion

                return Ok(new ResultResponse(true, phoneNo + totpType + "MobileConfirmed"));
            }


            return BadRequest(ResultResponse);
        }

        [HttpPost("ConfirmTotp/{clientName}")]
        public async Task<ActionResult> ConfirmTotp(string clientName, [FromBody] TotpConfirmationCodeRequest input)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            #endregion

            var phoneNo = input.PhoneNo;
            var typeTotp = input.TotpType;
            var isCovertToTotpTyp = Enum.TryParse(typeTotp, out TotpTypeCode totpType);
            if (!isCovertToTotpTyp) return BadRequest(new ResultResponse(false, MessageRes.CodeExpire));

            var ResultResponse = await _totpCode.ConfirmTotpCodeAsync(phoneNo, input.Code, totpType);
            if (ResultResponse.Successed)

            {
                var tempConfirmTotp = new TempConfirmTotp(phoneNo, totpType);
                _memoryCache.Set(phoneNo + totpType + "MobileConfirmed", tempConfirmTotp, TimeSpan.FromMinutes(4));

                return Ok(new ResultResponse(true, MessageRes.OKTotpInput,
                    new { keyConfirmed = phoneNo + totpType + "MobileConfirmed" }));
            }

            return BadRequest(ResultResponse);
        }

        [HttpPost("ConfirmAccountPhone/{clientName}")]
        public async Task<ActionResult> ConfirmAccountPhone(string clientName, [FromBody] string keyConfirmed)
        {
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            


            var confirmed = _memoryCache.Get<TempConfirmTotp>(keyConfirmed);
            if (!confirmed.HasValue) return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));

            if (confirmed.Value.TypeTotp == TotpTypeCode.TotpAccountConfirmationCode)
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == confirmed.Value.PhoneNo);
                if (user != null)
                {
                    var tokenPhone =
                        await _userManager.GenerateChangePhoneNumberTokenAsync(user, confirmed.Value.PhoneNo);
                    var confirmMoleNumber =
                        await _userManager.ChangePhoneNumberAsync(user, confirmed.Value.PhoneNo, tokenPhone);
                    //signin user    
                    await _signInManager.SignInAsync(user, false);

                    return Ok(await LoginJwt(user, client.LoginType));
                }

                return NotFound($"Unable to load user with ID '{confirmed.Value.PhoneNo}'.");
            }

            return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));
        }

        [HttpPost("ConfirmAccoutEmail/{clientName}")]
        public async Task<ActionResult> ConfirmAccoutEmail(string clientName, string code, string email)
        {
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                // return NotFound();
                return NotFound($"Unable to load user with ID '{email}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);


            if (!result.Succeeded) return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));
            await _signInManager.SignInAsync(user, false);

            return Ok(await LoginJwt(user, client.LoginType));
        }

        [HttpPost("RestPasswordByPhone/{clientName}")]
        public async Task<ActionResult> RestPasswordByPhone(string clientName, string keyConfirmed,
            [FromBody] ChangePasswordRequest input)
        {
            #region Check input

            if (string.IsNullOrEmpty(clientName))
                return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            #endregion

          

            var confirmed = _memoryCache.Get<TempConfirmTotp>(keyConfirmed);
            if (!confirmed.HasValue) return BadRequest(new ResultResponse(false, MessageRes.UnkonwnError));

            if (confirmed.Value.TypeTotp == TotpTypeCode.TotpAccountPasswordResetCode)
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == confirmed.Value.PhoneNo);
                if (user != null)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, code, input.Password);
                    //signin user    
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);

                        return Ok(await LoginJwt(user, client.LoginType));
                    }

                    var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                    return BadRequest(new ResultResponse(false, errorList));
                }

                return NotFound($"Unable to load user with ID '{confirmed.Value.PhoneNo}'.");
            }

            return NotFound(new ResultResponse(false, MessageRes.UnkonwnError));
        }

        [HttpPost("RestPasswordByEmail/{clientName}")]
        public async Task<ActionResult> RestPasswordByEmail(string clientName, string code, string email,
            [FromBody] ChangePasswordRequest input)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                // return NotFound();
                return NotFound($"Unable to load user with ID '{email}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ResetPasswordAsync(user, code, input.Password);


            if (!result.Succeeded)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ResultResponse(false, errorList));
            }

            await _signInManager.SignInAsync(user, false);

            return Ok(await LoginJwt(user, client.LoginType));
        }

        [HttpPost("SignOut/{clientName}")]
       
        [Authorize]
        public async Task<ActionResult> SignOut(string clientName)
        {
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete("Access-TokenSession", new CookieOptions
            {
                Secure = true
            });
            Response.Cookies.Delete("Refresh-TokenSession", new CookieOptions
            {
                Secure = true
            });
            var userId = HttpContext.User.FindFirstValue("id");
            await _refreshTokenRepository.DeleteRefreshTokenByuserIdAsync(userId);
            return Ok(new ResultResponse(true, "logout"));
        }

        [HttpPost("RefreshTokent/{clientName}")]
        public async Task<ActionResult> RefreshTokent(string clientName, [FromBody] string? refreshToken)
        {
            var client = InitialClients.GetClients().SingleOrDefault(c => c.ClientName == clientName.ToUpper());
            if (client == null) return BadRequest(new ResultResponse(false, MessageRes.ClientNoExist));

            var isRefreshTokenValid = false;
            var validToken = "";
            if (!string.IsNullOrEmpty(refreshToken) &&
                client is { LoginType: LoginType.Token or LoginType.TokenAndCookie })
            {
                isRefreshTokenValid = _tokenValidators.Validate(refreshToken);
                if (isRefreshTokenValid) validToken = refreshToken;
            }

            if (string.IsNullOrEmpty(refreshToken) &&
                client is { LoginType: LoginType.Cookie or LoginType.TokenAndCookie })
            {
                var isRefreshTokenInCookie =
                    HttpContext.Request.Cookies.TryGetValue("Refresh-TokenSession", out var refreshTokenCookie);
                if (isRefreshTokenInCookie)
                {
                    isRefreshTokenValid = _tokenValidators.Validate(refreshTokenCookie);
                    if (isRefreshTokenValid) validToken = refreshTokenCookie;
                }
            }

            if (!string.IsNullOrEmpty(validToken))
            {
                var userId = await _refreshTokenRepository.GetUserIdByRefreshTokenAsync(validToken);
                if (string.IsNullOrEmpty(userId))
                    return NotFound(new ResultResponse(false, MessageRes.InvalidRefreshToken));

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound(new ResultResponse(false, MessageRes.InvalidRefreshToken));

                return await LoginJwt(user, client.LoginType);
            }


            return BadRequest(new ResultResponse(false, MessageRes.InvalidRefreshToken));
        }

        private async Task<ActionResult> LoginJwt(ApplicationUser user, LoginType loginType)
        {
            var authClaims = _claimsGenrators.CreatClaims(user);

            var token = _tokenGenrator.GetAccessToken(authClaims);
            await _refreshTokenRepository.DeleteRefreshTokenByuserIdAsync(user.Id);
            await _refreshTokenRepository.WritRefreshTokenAsync(user.Id, token.RefreshToken);
            // string  tokenSerialized = JsonSerializer.Serialize<UserTokenResponse>(token);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };
           // var ticket = new AuthenticationTicket(_tokenValidators.GetClaimPrincipalValidatedToken(token.AccessToken), "JwtCookie");
          //  AuthenticateResult.Success(ticket);
           // await Request.HttpContext.SignInAsync("Identity.Application", _tokenValidators.GetClaimPrincipalValidatedToken(token.AccessToken), authProperties);
           // HttpContext.User = _tokenValidators.GetClaimPrincipalValidatedToken(token.AccessToken);
         switch (loginType)
            {
                case LoginType.TokenAndCookie:
                    Response.Cookies.Append("Access-TokenSession", token.AccessToken,
                        CookiesOptions.SetCookieOptions(token.Expiration));
                    Response.Cookies.Append("Refresh-TokenSession", token.RefreshToken,
                        CookiesOptions.SetCookieOptions(token.Expiration));
                    return Ok(new ResultResponse(true, MessageRes.UserLogin, token));
                case LoginType.Cookie:
                    Response.Cookies.Append("Access-TokenSession", token.AccessToken,
                        CookiesOptions.SetCookieOptions(token.Expiration));
                    Response.Cookies.Append("Refresh-TokenSession", token.RefreshToken,
                        CookiesOptions.SetCookieOptions(token.Expiration));
                    return Ok(new ResultResponse(true, MessageRes.UserLogin));

                case LoginType.Token:
                    return Ok(new ResultResponse(true, MessageRes.UserLogin, token));
            }

            return Ok(new ResultResponse(false, MessageRes.WrongLoginType));
        }

        [HttpPost("test")]
        [Authorize]
        public async Task<ActionResult> Test(LoginRequest login)
        {

            var user = User.Identity.IsAuthenticated;
            var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(user+$"userid={userId}");
        }
        [HttpPost("test2")]
        public async Task<ActionResult> Test2(string userId)
        {

            //var user = User.Identity.IsAuthenticated;
            //var userId= HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //return Ok(user+$"userid={userId}");
            // var x=typeof(IEasyCachingProviderBase).IsAssignableFrom(typeof(DefaultInMemoryCachingProvider));
            //var y=typeof(IEasyCachingProviderBase).IsAssignableFrom(typeof(HybridCachingProvider));



            //var userLoginPolicy = new UserLoginPolicyOptions();
            //userLoginPolicy.PolicyName = "d2";
            //await _UOW.LoginOptionRepository.InsertAsync(userLoginPolicy);
            //_UOW.Save();

            try
            {
                var user =await _userManager.FindUserByEmailOrPhoneAsync("989123406285");
                var policyOption = await _UOW.LoginOptionRepository.GetByAsync("d2");
                var userlog = new ApplicationUserPolicy(user.Id, policyOption.Id);
                await _UOW.UserLoginPolicyRepository.AddPolicyToUserAsync(userlog);
                _UOW.Save();

                var policyId= await _UOW.UserLoginPolicyRepository.GetPolicyIdByUserIdAsync(user.Id);
        
                var policy = await _UOW.UserLoginPolicyRepository.GetPolicyByUserIdAsync(user.Id);
                var userlist = await _UOW.UserLoginPolicyRepository.GetAllUserWithPolicyIdAsync(userlog.PolicyId);
            }
            catch (Exception e)
            {
                return BadRequest(ExceptionMessage.GetPerisanSqlExceptionMessage(e));
            }

           return Ok();
        }

        #region CTOR

        private readonly UnitOfWork _UOW;
        private readonly UserManagementService _userManager;
        private readonly ITotpCode _totpCode;
        private readonly ILogger<RegisterRequest> _logger;
        private readonly  IEasyCachingProviderBase _memoryCache;
       
        private readonly IMailCode _mailCode;
        private readonly IDataProtector _protector;
        private readonly TotpSettings _totpSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthClaimsGenrators _claimsGenrators;
        private readonly ITokenGenrators _tokenGenrator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenValidators _tokenValidators;

        public AccountController(UserManagementService userManager,
            ITotpCode totpCode, ILogger<RegisterRequest> logger,
            IMailCode mailCode,  IEasyCachingProviderBase memoryCache,
            IOptions<TotpSettings> options,
            IDataProtectionProvider dataProtectionProvider, DataProtectionPepuseString dataProtectionPepuseString,
            SignInManager<ApplicationUser> signInManager, IAuthClaimsGenrators claimsGenrators,
            ITokenGenrators tokenGenrator, IRefreshTokenRepository refreshTokenRepository,
            ITokenValidators tokenValidators, UnitOfWork uow)
        {
            _userManager = userManager;
            _totpCode = totpCode;
            _logger = logger;
            _mailCode = mailCode;
            _memoryCache = memoryCache;
            _signInManager = signInManager;
            _claimsGenrators = claimsGenrators;
            _tokenGenrator = tokenGenrator;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenValidators = tokenValidators;
            _UOW = uow;
            _totpSettings = options?.Value ?? new TotpSettings();
            _protector = dataProtectionProvider.CreateProtector(dataProtectionPepuseString.PhoneNoInCooki);
        }

        #endregion
    }
}