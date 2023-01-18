using System.ComponentModel.DataAnnotations;

namespace Jwt.Identity.Framework.DataAnnotations
{
    public class PhoneOrEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var valueAsString = value.ToString();
                if (string.IsNullOrEmpty(valueAsString)) return ValidationResult.Success;

                var emailValidtionAttribute = new EmailAddressAttribute();
                var mobilValidation = new MobileNoAttribute();

                if (emailValidtionAttribute.IsValid(valueAsString) || mobilValidation.IsValid(valueAsString))
                    return ValidationResult.Success;

                return new ValidationResult("ایمیل یا شماره موبایل وارد شده معتبر نمیباشد",
                    new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}