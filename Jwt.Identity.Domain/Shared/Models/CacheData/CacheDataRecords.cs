using System;
using Jwt.Identity.Domain.IServices.Totp.Enum;

namespace Jwt.Identity.Domain.Shared.Models.CacheData
{
    public record TempConfirmTotp(string PhoneNo, TotpTypeCode TypeTotp);

    public record TempConfirmEmail(string PhoneNo, TotpTypeCode TypeTotp);

    public record TotpTempData(string UserMobileNo, DateTime ExpirationTime, byte[] SecretKey);

    public record TempIpBlock(string IpAddress, DateTime ExpirationTime);

    public record ValidTokenCacheModel(string Token, string UserId);
}