using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ReportWorkTime.Models
{
    public class GoingOut
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int GoingOutId { get; set; }

        public string EmpUserId { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime? DateOfAbsence { get; set; }

        
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:HH\:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartAbsence { get; set; }

        
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:HH\:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndAbsence { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        public TimeSpan HoursOfAbsence { get; set; }

        public EmployeeUser? User { get; set; }

        public TypeOfDay TypeOfDay { get; set; }

        [Range(1, 30)]
        public int? NumberOfDays { get; set; }
    }
}
