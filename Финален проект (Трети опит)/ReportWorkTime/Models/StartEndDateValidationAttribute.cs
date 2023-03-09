using System.ComponentModel.DataAnnotations;

namespace ReportWorkTime.Models
{
    public class StartEndDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (Absence)validationContext.ObjectInstance;

            if (model.DateOfAbsence != model.EndDateOfAbsence)
            {
                return new ValidationResult("The start and end dates must be the same.");
            }

            return ValidationResult.Success;
        }
    }
}
