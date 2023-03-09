using System.ComponentModel.DataAnnotations;

namespace ReportWorkTime.Models
{
    public class StartEndTimeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (Absence)validationContext.ObjectInstance;

            if (model.StartAbsence > model.EndAbsence)
            {
                return new ValidationResult("Start time cannot be greater than end time.");
            }

            return ValidationResult.Success;
        }
    }
}
