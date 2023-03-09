using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkTimeReport.Models;

namespace WorkTimeReport.Controllers
{
    public class WorkTimeController : Controller
    {
        private UserManager<EmployeeUser> _userManager;
        private AppDbContext _context;

        public EmployeeUser User { get; set; }
        public WorkTimeController(UserManager<EmployeeUser> usrManager, AppDbContext context)
        {
            _userManager = usrManager;
            _context = context;
        }
        public async Task<IActionResult> Index(string? id)
        {
            if(id == null)
            {
                User = await _userManager.GetUserAsync(HttpContext.User);
                if(User == null)
                {
                    return NotFound();
                }
            }
            else
            {
                User = await _userManager.FindByIdAsync(id);
                if (User == null)
                {
                    return NotFound();
                }
            }

            var currentMonth = DateTime.Today.Month;
            var startDate = new DateTime(DateTime.Today.Year, currentMonth, 1);
            var endDate = new DateTime(DateTime.Today.Year, currentMonth, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            ViewBag.month = startDate;
            User.WorkingTimes = _context.WorkingTimes
                .Where(w => w.EmpUserId == User.Id && w.Date >= startDate && w.Date <= endDate)
                .ToList();

            User.GoingOuts = _context.GoingOuts
                .Where(g => g.EmpUserId == User.Id && g.DateOfAbsence >= startDate && g.DateOfAbsence <= endDate)
                .ToList();

            

            return View(User);
        }
    }
}
