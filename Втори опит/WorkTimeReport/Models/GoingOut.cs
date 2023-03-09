using System.ComponentModel.DataAnnotations;

namespace WorkTimeReport.Models
{
    public class GoingOut
    {
        [Key]
        public string Id { get; set; }

        public string EmpUserId { get; set; }
        
        //[Required]
        [DataType(DataType.Date)]
        public DateTime? DateOfAbsence { get; set; }

        //[Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartAbsence { get; set; }

        //[Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndAbsence { get; set; }

        public TimeSpan HoursOfAbsence { get; set; }

        [StringLength(1000)]
        public string Reason { get; set; }

        public EmployeeUser User { get; set; }
    }
}
