using System.ComponentModel.DataAnnotations;

namespace WorkTimeReport.Models
{
    public class WorkingTime
    {
        [Key]
        public string Id { get; set; }

        public string EmpUserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartWork { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndWork { get; set; }

        public TypeOfDay TypeOfDay { get; set; }

        public TimeSpan HoursForDay { get; set; }

        public EmployeeUser User { get; set; }
    }
}
