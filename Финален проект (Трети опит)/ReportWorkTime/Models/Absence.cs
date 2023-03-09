using System.ComponentModel.DataAnnotations;

namespace ReportWorkTime.Models
{
    public class Absence
    {
        public string EmpUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        //[StartEndDateValidation]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DateOfAbsence { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [StartEndDateValidation]
        //[StartEndDateValidation]//(ErrorMessage = "The start and end dates must be the same.")
        public DateTime EndDateOfAbsence { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:HH\:mm}")]
        public DateTime StartAbsence { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:HH\:mm}")]
        [StartEndTimeValidation(ErrorMessage = "Start time cannot be greater than end time.")]
        public DateTime EndAbsence { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        //public EmployeeUser? EmpUser { get; set; }

        public TypeOfDay? Type { get; set; }

        [Range(1, 30)]
        public int? NumberOfDays { get; set; }

        //public TimeSpan HoursOfAbsence { get; set; }
    }
}
