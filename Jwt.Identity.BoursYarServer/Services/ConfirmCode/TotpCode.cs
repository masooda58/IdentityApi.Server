using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using Jwt.Identity.BoursYarServer.Models.SettingModels;
using Jwt.Identity.BoursYarServer.Resources;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Domain.Shared.Models.CacheData;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Framework.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Jwt.Identity.BoursYarServer.Services.ConfirmCode
{
    public class TotpCode : ITotpCode
    {
        public async Task<ResultResponse> SendTotpCodeAsync(string phoneNo, TotpTypeCode sendType)
        {
            #region Ip Block for send sms

            var remoteIpAddress = _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress;
            var ipBlock = _memoryCache.Get<TempIpBlock>(remoteIpAddress!.ToString());

            #endregion

            //آیا کد ریست قبلا ارسال شده است
            var resetPasswordTotp =
                _memoryCache.Get<TotpTempData>(phoneNo + sendType);
            if (resetPasswordTotp.HasValue)
                //  اگر هنوز کد ارسالی معتبر بود
                //نیاز به این شرط نیست چون اگر زمان منقضی شده باشد خود به خود از کش پاک می شود
                if (DateTime.Now < resetPasswordTotp.Value.ExpirationTime)
                {
                    var remainTime = (int)(resetPasswordTotp.Value.ExpirationTime - DateTime.Now).TotalSeconds;
                    return new ResultResponse(false, $"{remainTime}");
                }

            if (ipBlock.HasValue)
            {
                var remainTime = (int)(ipBlock.Value.ExpirationTime - DateTime.Now).TotalSeconds;
                return new ResultResponse(false, $"{remainTime}");
            }


            var secretKey = _totp.CreateSecretKey();
            var totpCode = _totp.GenerateTotp(secretKey);

            var totpTemp = new TotpTempData(SecretKey: secretKey, UserMobileNo: phoneNo,
                ExpirationTime: DateTime.Now.AddSeconds(_options.Step));
            var tempIpBlock = new TempIpBlock(remoteIpAddress!.ToString(),
                DateTime.Now.AddSeconds(_options.Step - 2));
            // var memoryCacheOptions = new MemoryCacheEntryOptions()
            //    .SetSlidingExpiration(TimeSpan.FromSeconds(_options.Step));
            await _memoryCache.SetAsync(phoneNo + sendType, totpTemp,
                TimeSpan.FromSeconds(_options.Step));
            // block ip for send sms in step time
            await _memoryCache.SetAsync(remoteIpAddress!.ToString(), tempIpBlock,
                TimeSpan.FromSeconds(_options.Step));
            var tempData = _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext);
            tempData["PhoneNo"] = phoneNo;
            //جهت تست
            if (_env.IsDevelopment())
            {
                await _emailSender.SendEmailAsync("test@test.com", "sms to " + phoneNo, totpCode);
                return new ResultResponse(true, "");
            }

            await _smsSender.SendSmsAsync(phoneNo, totpCode, "شرکت فلان");
            return new ResultResponse(true, "");
        }

        public async Task<ResultResponse> ConfirmTotpCodeAsync(string phoneNo, string code, TotpTypeCode confirmType)
        {
            var resetPasswordTotp =
                _memoryCache.Get<TotpTempData>(phoneNo + confirmType);

            if (!resetPasswordTotp.HasValue || DateTime.Now > resetPasswordTotp.Value.ExpirationTime)
                //  اگر هنوز کد ارسالی معتبر بود
                //نیاز به این شرط نیست چون اگر زمان منقضی شده باشد خود به خود از کش پاک می شود


                return new ResultResponse(false, ErrorMessageRes.CodeExpire);

            var mathResult = _totp.VerifyTotp(resetPasswordTotp.Value.SecretKey, code);
            //اگر کد درست باشد
            if (mathResult.Successed) return new ResultResponse(true, "");

            // اگر کد غلط باشد

            return new ResultResponse(false, ErrorMessageRes.WrongTotpInput);
        }

        #region Ctor

        private readonly  IHybridCachingProvider _memoryCache;
        private readonly IPhoneTotpProvider _totp;
        private readonly TotpSettings _options;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;
        private readonly ISmsSender _smsSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public TotpCode( IHybridCachingProvider memoryCache, IPhoneTotpProvider totp, IOptions<TotpSettings> options,
            IEmailSender emailSender, IWebHostEnvironment env, ISmsSender smsSender,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
            ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _memoryCache = memoryCache;
            _totp = totp;
            _emailSender = emailSender;
            _env = env;
            _smsSender = smsSender;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            ;
            _options = options?.Value ?? new TotpSettings();
        }

        #endregion
    }
}