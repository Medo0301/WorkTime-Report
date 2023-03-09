using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ReportWorkTime.Models
{
    public class EmployeeUser : IdentityUser
    {
        [Required]
        [RegularExpression(@"^([A-Za-z]+\s)*[A-Za-z]+$|^[А-Я][а-я]{0,48}(\s[А-Я][а-я]{0,48})?$")]
        [StringLength(40, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^([A-Za-z]+\s)*[A-Za-z]+$|^[А-Я][а-я]{0,48}(\s[А-Я][а-я]{0,48})?$")]
        [StringLength(40, MinimumLength = 1)]
        public string LastName { get; set; }

        public bool IsWorking { get; set; }

        public ICollection<GoingOut> GoingOuts { get; set; }

        public ICollection<WorkingDay> WorkingDays { get; set; }

        //public int SelectedMonth { get; set; }
        //public int SelectedYear { get; set; }
    }
}
