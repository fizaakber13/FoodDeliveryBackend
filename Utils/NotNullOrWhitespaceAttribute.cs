using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.Utils
{
    public class NotNullOrWhitespaceAttribute : ValidationAttribute
    {
        public NotNullOrWhitespaceAttribute()
        {
            ErrorMessage = "The {0} field is required and cannot be whitespace.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success!;
        }
    }
}
