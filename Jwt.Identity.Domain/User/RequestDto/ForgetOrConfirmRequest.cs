using System.ComponentModel.DataAnnotations;
using Jwt.Identity.Framework.DataAnnotations;
using Jwt.Identity.Framework.Extensions;

namespace Jwt.Identity.Domain.User.RequestDto
{
    public class ForgetOrConfirmRequest
    {
        private string _normalEmailOrPhone;

        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        // [EmailAddress(ErrorMessage = "{0} وارد شده صحیح نمی باشد")]
        [PhoneOrEmail]
        [Display(Name = "ایمیل یا شماره موبایل")]
        // ErrorMessage = "ایمیل یا شماره موبایل  قبلا  در سایت ثبت نام کرده است",
        public string EmailOrPhone
        {
            get => _normalEmailOrPhone.ToNormalPhoneNo();
            set => _normalEmailOrPhone = value;
        }
    }
}