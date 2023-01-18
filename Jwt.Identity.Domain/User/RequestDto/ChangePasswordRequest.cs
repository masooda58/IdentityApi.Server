using System.ComponentModel.DataAnnotations;

namespace Jwt.Identity.Domain.User.RequestDto
{
    public class ChangePasswordRequest
    {
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