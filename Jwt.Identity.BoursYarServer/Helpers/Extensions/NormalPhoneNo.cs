using Jwt.Identity.Framework.DataAnnotations;

namespace Jwt.Identity.BoursYarServer.Helpers.Extensions
{
    public static class NormalPhoneNo
    {
        public static string ToNormalPhoneNo(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var checkMobileNoAttribute = new MobileNoAttribute();
            var isMobileNo = checkMobileNoAttribute.IsValid(value);
            if (isMobileNo)
                //// بود 9رقم سمت راست جدا می شود Valid با توجه به

                return "989" + value.Substring(value.Length - 9);

            return value;
        }
    }
}