using System.ComponentModel.DataAnnotations;
using Jwt.Identity.Framework.DataAnnotations;

namespace Jwt.Identity.Domain.IServices.Totp.Request
{
    public class TotpConfirmationCodeRequest
    {
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [Display(Name = "کد ارسالی")]
        public string Code { get; set; }

        [MobileNo]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [Display(Name = "شماره موبایل")]
        public string? PhoneNo { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [Display(Name = "نوع ")]
        public string? TotpType { get; set; }
    }
}