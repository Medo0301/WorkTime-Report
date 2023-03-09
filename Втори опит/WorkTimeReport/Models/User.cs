using System.ComponentModel.DataAnnotations;

namespace WorkTimeReport.Models
{
    public class User
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^([A-Za-z]+\s)*[A-Za-z]+$|^[А-Я][а-я]{0,48}(\s[А-Я][а-я]{0,48})?$")]
        [StringLength(40, MinimumLength = 1)]
        public string FName { get; set; }

        [Required]
        [RegularExpression(@"^([A-Za-z]+\s)*[A-Za-z]+$|^[А-Я][а-я]{0,48}(\s[А-Я][а-я]{0,48})?$")]
        [StringLength(40, MinimumLength = 1)]
        public string LName { get; set; }

        public bool IsWorking { get; set; }

        //public ICollection<GoingOut> GoingOuts { get; set; }

        //public ICollection<WorkingTime> WorkTimes { get; set; }
    }
}

