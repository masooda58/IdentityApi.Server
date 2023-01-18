using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Jwt.Identity.Framework.DataAnnotations
{
    public class MobileNoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var valueAsString = value.ToString();
                if (string.IsNullOrEmpty(valueAsString)) return ValidationResult.Success;
                // const string mobPattern = @"^((\+989|9|09|989|00989)(01|02|03|04|05|10|11|12|13|14|15|16|17|18|19|20|21|22|30|31|32|33|34|35|36|37|38|39|90|91|92|93|94))(\d{7})$";
                const string mobPattern = @"^(9|09|00989|989|\+989)(\d{9})$";
                var isValidPhone = Regex.IsMatch(valueAsString, mobPattern);

                if (isValidPhone) return ValidationResult.Success;

                return new ValidationResult("شماره موبایل وارد شده معتبر نیست");
            }

            return ValidationResult.Success;
        }
    }
}