using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReportWorkTime.Models;
using System.Data;
using System.Diagnostics;

namespace ReportWorkTime.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<EmployeeUser> _userManager;
        private AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<EmployeeUser> userManager, AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            //List<SelectListItem> emp = new List<SelectListItem>();
            //emp = _context.Users.Select(x => new SelectListItem { 
            //        Text = x.FirstName + " " + x.LastName + ", " + x.Email, Value = x.Id})
            //        .ToList();
            //ViewBag.Users = emp.OrderBy(s => s.Text);
            
            
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
                    Errors(result);
                }
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string employeeId)
        {
            if (employeeId == null)
            {
                return NotFound();
            }

            EmployeeUser user = await _userManager.FindByIdAsync(employeeId);
                    
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            return View("Index", _userManager.Users);

            ////var employee = _context.Users.Find(id);

            ////if (employee == null)
            ////{
            ////    return NotFound();
            ////}

            ////_context.Users.Remove(employee);
            ////_context.SaveChanges();

            ////return RedirectToAction("Index");
            //EmployeeUser user = await _userManager.FindByIdAsync(id);
            //if (user == null)
            //{
            //    return NotFound();
            //}

            
            ////{
            //IdentityResult result = await _userManager.DeleteAsync(user);
            //    if (!result.Succeeded)
            //    {
            //        Errors(result);
            //    }

            //return View("Index");
            ////}
            ////else
            ////{
            ////    ModelState.AddModelError("", "User Not Found");
            ////}
            ////return View("Index", _userManager.Users);
        }

        [HttpPost]
        public async Task<IActionResult> StartEndWorking(string id)
        {
            EmployeeUser user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                if(user.IsWorking)
                {
                    WorkingDay workDay = _context.WorkingDays
                        .Where(w => w.EmpUserId == user.Id && w.EndWork == new DateTime(01, 01, 01, 0, 0, 0))
                        .OrderByDescending(w => w.StartWork)
                        .FirstOrDefault();
                    //if (workDay == null)
                    //{
                    //    return NotFound();
                    //}

                    workDay.EndWork = DateTime.Now;
                    workDay.HoursForDay = workDay.EndWork.Subtract(workDay.StartWork);

                    _context.Update(workDay);
                    user.IsWorking = false;
                    _context.Update(user);

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    WorkingDay workDay = new WorkingDay
                    {
                        EmpUserId = user.Id,
                        Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day),
                        StartWork = DateTime.Now,
                        //EndWork = DateTime.Now,
                        TypeOfDay = TypeOfDay.WorkingDay
                    };

                    _context.Add(workDay);
                    user.IsWorking = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            return View("Index", _userManager.Users);
        }
        //[HttpPost]
        //public async Task<IActionResult> StartWorking(string id)
        //{
        //    EmployeeUser user = await _userManager.FindByIdAsync(id);

        //    if (user != null)
        //    {
        //        WorkingDay workDay = new WorkingDay
        //        {
        //            EmpUserId = user.Id,
        //            Date = DateTime.Now,
        //            StartWork = DateTime.Now,
        //            TypeOfDay = TypeOfDay.WorkingDay
        //        };

                
        //        _context.Add(workDay);
        //        user.IsWorking = true;
        //        _context.Update(user);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> EndWorking(string id)
        //{
            
        //    EmployeeUser user = await _userManager.FindByIdAsync(id);

        //    if (user != null)
        //    {
        //        WorkingDay workDay = _context.WorkingDays
        //            .Where(w => w.EmpUserId == user.Id && w.EndWork == null)
        //            .OrderByDescending(w => w.StartWork)
        //            .FirstOrDefault();
        //        if (workDay == null)
        //        {
        //            return NotFound();
        //        }

        //        workDay.EndWork = DateTime.Now;
        //        workDay.HoursForDay = workDay.EndWork.Subtract(workDay.StartWork);
                    
        //        _context.Update(workDay);
        //        user.IsWorking = false;
        //        _context.Update(user);

        //        await _context.SaveChangesAsync();

        //        return View("Index", _userManager.Users);

        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}