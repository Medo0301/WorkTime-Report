using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorkTimeReport.Models;

namespace WorkTimeReport.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<EmployeeUser> _userManager;
        private AppDbContext _context;
        private string? WorkTimeId { get; set; }

        public HomeController(ILogger<HomeController> logger, UserManager<EmployeeUser> userManager, AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        [Authorize(Roles = "Admin")]
        public ViewResult Create() => View();


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                EmployeeUser empUser = new EmployeeUser
                {
                    FirstName = user.FName,
                    LastName = user.LName,
                    Email = user.Email,
                    UserName = user.UserName,
                    IsWorking = false
                };

                IdentityResult result = await _userManager.CreateAsync(empUser, user.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> StartWorking(string id)
        {
            EmployeeUser user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                WorkingTime workTime = new WorkingTime
                {
                    EmpUserId = user.Id,
                    Date = DateTime.Now,
                    StartWork = DateTime.Now,
                    TypeOfDay = TypeOfDay.WorkingDay
                };

                WorkTimeId = workTime.Id;
                _context.Add(workTime);
                user.IsWorking = true;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EndWorking(string id)
        {
            EmployeeUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                WorkingTime workTime = _context.WorkingTimes
                    .FirstOrDefault(w => w.EmpUserId == user.Id && w.Id == WorkTimeId);
                if (workTime != null)
                {
                    workTime.EndWork = DateTime.Now;
                    workTime.HoursForDay = 
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}