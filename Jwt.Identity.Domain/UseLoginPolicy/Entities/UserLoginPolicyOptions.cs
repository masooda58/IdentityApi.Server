using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Jwt.Identity.Domain.UseLoginPolicy.Enum;
using Jwt.Identity.Domain.User.Entities;

namespace Jwt.Identity.Domain.UseLoginPolicy.Entities
{
    public class UserLoginPolicyOptions
    {
        [Key]
        public string Id { get; set; }=Guid.NewGuid().ToString();

 
        public string PolicyName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [Display(Name = "تعداد ورود مجاز ")]
        public int NumberOfLogin { get; set; } = 1;
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [Display(Name = "خط مشی ورود درصورت عدم رعایت تعداد ورود مجاز ")]
        public MultiUserNumberPolicy OvereNumberOfLogin { get; set; } = MultiUserNumberPolicy.SignOutLastLogin;
        // اینجا می توان محدودیت های دیگر مثل محدود کردن رنج ای پی هم اضافه کرد
      
        public virtual ICollection<ApplicationUserPolicy> ApplicationUserPolicies { get; set; }
    }


}
