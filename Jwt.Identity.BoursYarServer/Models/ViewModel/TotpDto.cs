using System.ComponentModel.DataAnnotations;
using Jwt.Identity.BoursYarServer.Helpers.Extensions;
using Jwt.Identity.Domain.IServices.Totp.Enum;
using Jwt.Identity.Framework.DataAnnotations;

namespace Jwt.Identity.BoursYarServer.Models.ViewModel
{
    public class TotpDto
    {
        private string _normalEmailOrPhone;

        [Required]
        [MobileNo]
        public string PhoneNumber
        {
            get => _normalEmailOrPhone.ToNormalPhoneNo();
            set => _normalEmailOrPhone = value;
        }

        [Required] public TotpTypeCode TotpType { get; set; }
    }
}