using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.IdentityPolicy.Enum;

namespace Jwt.Identity.Domain.IdentityPolicy.Entity
{
   public class IdentitySettingPolicy
   {
       [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        [Display(Name = "کلمه عبور شامل عدد باشد")]
        public bool RequireDigit { get; set; } = false;
        [Display(Name = "حداقل طول کلمه عبور")]
        [Range(5,Int32.MaxValue,ErrorMessage = "مقدار {0} بزرگتر از {1} باشد")]
        public int RequiredLength{ get; set; } = 1;
        [Display(Name = "کلمه عبور شامل کارکتر غیر حروف و عدد")]
        public bool RequireNonAlphanumeric { get; set; }= false;
        [Display(Name = "کلمه عبور شامل حروف بزرگ باشد")]
        public bool RequireUppercase{ get; set; } = false;
        [Display(Name = "کلمه عبور شامل حروف کوچک باشد")]
        public bool RequireLowercase{ get; set; } = false;
        [Display(Name = "تعداد کارکتر منحصر به فرد در گذر واژه")]
        [Range(0,Int32.MaxValue,ErrorMessage = "مقدار {0} بزرگتر از {1} باشد")]
        public int RequiredUniqueChars { get; set; } = 1;
        [Range(15,Int32.MaxValue,ErrorMessage = "مقدار {0} بزرگتر از {1} باشد")]
        [Display(Name = "مدت زمان قفل یوزر کاربری")]
        public int DefaultLockoutTimeSpanMinute { get; set; } = 30;
        [Range(2,Int32.MaxValue,ErrorMessage = "مقدار {0} بزرگتر از {1} باشد")]
        [Display(Name = "تعداد خطای  مجاز ورود اطلاعات")]
        public int MaxFailedAccessAttempts { get; set; }= 3;
        public bool RequireConfirmedAccount{ get; set; } = false;
        [Range(1,Int32.MaxValue,ErrorMessage = "مقدار {0} بزرگتر از {1} باشد")]
        [Display(Name = "زمان ارسال توکن های ارسالی")]
        public int TokenLifespanHour { get; set; } = 8;
        [Range(1,5,ErrorMessage = "{0} حداقل {2} و حداکثر {1} باشد")]
        [Display(Name = "زمان اعتبار کد پیامکی")]
        public int TotpLifeSpanMinute { get; set; } = 2;
        [Display(Name = "نمایش Captcha")]
        public CaptchStrategy CaptchStrategy { get; set; } = CaptchStrategy.Never;
    }
}
