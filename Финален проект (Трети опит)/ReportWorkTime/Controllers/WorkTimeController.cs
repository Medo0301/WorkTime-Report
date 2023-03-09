using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ReportWorkTime.Models;
using System.Data;

namespace ReportWorkTime.Controllers
{
    public class WorkTimeController : Controller
    {
        //private int selectMonth = DateTime.Now.Month;
        //private int selectYear = DateTime.Now.Year;

        private UserManager<EmployeeUser> _userManager;
        private AppDbContext _context;

        
        public EmployeeUser User { get; set; }

        [BindProperty]
        public Absence Absence { get; set; }

        [BindProperty]
        public GoingOut GoingOut { get; set; }

        public WorkTimeController(UserManager<EmployeeUser> usrManager, AppDbContext context)
        {
            _userManager = usrManager;
            _context = context;
        }
        public async Task<IActionResult> Index(string? id)
        {
            if (id == null)
            {
                User = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
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

            int selectedMonth = DateTime.Now.Month;
            int selectedYear = DateTime.Now.Year;

            var startDate = new DateTime(selectedYear, selectedMonth, 1);
            var endDate = new DateTime(selectedYear, selectedMonth, DateTime.DaysInMonth(selectedYear, selectedMonth));
            ViewBag.month = startDate;

            User.WorkingDays = _context.WorkingDays
                .Where(w => w.EmpUserId == User.Id && w.Date >= startDate && w.Date <= endDate)
                .OrderBy(w => w.StartWork)
                .ToList();

            User.GoingOuts = _context.GoingOut
                .Where(g => g.EmpUserId == User.Id && g.DateOfAbsence >= startDate && g.DateOfAbsence <= endDate)
                .OrderBy(g => g.StartAbsence)
                .ToList();

            int? businessTrip = 0;
            int? paidLeave = 0;
            int? unpaidLeave = 0;
            int? sickLeave = 0;

            foreach (var goingOut in User.GoingOuts)
            {
                if(goingOut.TypeOfDay == TypeOfDay.BusinessTrip)
                {
                    businessTrip += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.PaidLeave)
                {
                    paidLeave += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.UnpaidLeave)
                {
                    unpaidLeave += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.SickLeave)
                {
                    sickLeave += goingOut.NumberOfDays;
                }

            }
            ViewBag.BusinessTrip = businessTrip;
            ViewBag.PaidLeave = paidLeave;
            ViewBag.UnpaidLeave = unpaidLeave;
            ViewBag.SickLeave = sickLeave;


            var absenceHours = User.GoingOuts.Sum(h => h.HoursOfAbsence.Ticks);
            var timeSpanAbsence = new TimeSpan(absenceHours);
            ViewBag.absenceHours = timeSpanAbsence;

            var totalHours = User.WorkingDays.Sum(h => h.HoursForDay.Ticks);
            var timeSpan = new TimeSpan(totalHours) - timeSpanAbsence;
            ViewBag.hours = timeSpan;

            HttpContext.Session.SetInt32("SelectedMonth", selectedMonth);
            HttpContext.Session.SetInt32("SelectedYear", selectedYear);

            return View(User);
        }

        
        public async Task<IActionResult> NextMonth(string id)
        {
            int selectedMonth = HttpContext.Session.GetInt32("SelectedMonth") ?? DateTime.Now.Month;
            int selectedYear = HttpContext.Session.GetInt32("SelectedYear") ?? DateTime.Now.Year;

            if (selectedYear > 9999 && selectedMonth > 12)
            {
                return RedirectToAction("Index");
            }

            if (selectedMonth == 12)
            {
                selectedMonth = 1;
                selectedYear++;
            }
            else
            {
                selectedMonth++;
            }

            HttpContext.Session.SetInt32("SelectedMonth", selectedMonth);
            HttpContext.Session.SetInt32("SelectedYear", selectedYear);

            User = await _userManager.FindByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            var startDate = new DateTime(selectedYear, selectedMonth, 1);
            var endDate = new DateTime(selectedYear, selectedMonth, DateTime.DaysInMonth(selectedYear, selectedMonth));
            ViewBag.month = startDate;

            User.WorkingDays = _context.WorkingDays
                .Where(w => w.EmpUserId == User.Id && w.Date >= startDate && w.Date <= endDate)
                .OrderBy(w => w.StartWork)
                .ToList();

            User.GoingOuts = _context.GoingOut
                .Where(g => g.EmpUserId == User.Id && g.DateOfAbsence >= startDate && g.DateOfAbsence <= endDate)
                .OrderBy(g => g.StartAbsence)
                .ToList();

            int? businessTrip = 0;
            int? paidLeave = 0;
            int? unpaidLeave = 0;
            int? sickLeave = 0;

            foreach (var goingOut in User.GoingOuts)
            {
                if (goingOut.TypeOfDay == TypeOfDay.BusinessTrip)
                {
                    businessTrip += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.PaidLeave)
                {
                    paidLeave += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.UnpaidLeave)
                {
                    unpaidLeave += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.SickLeave)
                {
                    sickLeave += goingOut.NumberOfDays;
                }

            }
            ViewBag.BusinessTrip = businessTrip;
            ViewBag.PaidLeave = paidLeave;
            ViewBag.UnpaidLeave = unpaidLeave;
            ViewBag.SickLeave = sickLeave;


            var absenceHours = User.GoingOuts.Sum(h => h.HoursOfAbsence.Ticks);
            var timeSpanAbsence = new TimeSpan(absenceHours);
            ViewBag.absenceHours = timeSpanAbsence;

            var totalHours = User.WorkingDays.Sum(h => h.HoursForDay.Ticks);
            var timeSpan = new TimeSpan(totalHours) - timeSpanAbsence;
            ViewBag.hours = timeSpan;

            return View("Index", User);
        }

        public async Task<IActionResult> PreviousMonth(string id)
        {
            
            int selectedMonth = HttpContext.Session.GetInt32("SelectedMonth") ?? DateTime.Now.Month;
            int selectedYear = HttpContext.Session.GetInt32("SelectedYear") ?? DateTime.Now.Year;

            if (selectedYear < 1900 && selectedMonth <= 1)
            {
                return RedirectToAction("Index");
            }

            if (selectedMonth == 1)
            {
                selectedMonth = 12;
                selectedYear--;
            }
            else
            {
                selectedMonth--;
            }
            

            HttpContext.Session.SetInt32("SelectedMonth", selectedMonth);
            HttpContext.Session.SetInt32("SelectedYear", selectedYear);

            User = await _userManager.FindByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            var startDate = new DateTime(selectedYear, selectedMonth, 1);
            var endDate = new DateTime(selectedYear, selectedMonth, DateTime.DaysInMonth(selectedYear, selectedMonth));
            ViewBag.month = startDate;

            User.WorkingDays = _context.WorkingDays
                .Where(w => w.EmpUserId == User.Id && w.Date >= startDate && w.Date <= endDate)
                .OrderBy(w => w.StartWork)
                .ToList();

            User.GoingOuts = _context.GoingOut
                .Where(g => g.EmpUserId == User.Id && g.DateOfAbsence >= startDate && g.DateOfAbsence <= endDate)
                .OrderBy(g => g.StartAbsence)
                .ToList();

            int? businessTrip = 0;
            int? paidLeave = 0;
            int? unpaidLeave = 0;
            int? sickLeave = 0;

            foreach (var goingOut in User.GoingOuts)
            {
                if (goingOut.TypeOfDay == TypeOfDay.BusinessTrip)
                {
                    businessTrip += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.PaidLeave)
                {
                    paidLeave += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.UnpaidLeave)
                {
                    unpaidLeave += goingOut.NumberOfDays;
                }
                if (goingOut.TypeOfDay == TypeOfDay.SickLeave)
                {
                    sickLeave += goingOut.NumberOfDays;
                }

            }
            ViewBag.BusinessTrip = businessTrip;
            ViewBag.PaidLeave = paidLeave;
            ViewBag.UnpaidLeave = unpaidLeave;
            ViewBag.SickLeave = sickLeave;


            var absenceHours = User.GoingOuts.Sum(h => h.HoursOfAbsence.Ticks);
            var timeSpanAbsence = new TimeSpan(absenceHours);
            ViewBag.absenceHours = timeSpanAbsence;

            var totalHours = User.WorkingDays.Sum(h => h.HoursForDay.Ticks);
            var timeSpan = new TimeSpan(totalHours) - timeSpanAbsence;
            ViewBag.hours = timeSpan;

            return View("Index", User);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.Canceled && !context.HttpContext.Request.Path.Value.Contains("WorkTime"))
            {
                HttpContext.Session.SetInt32("SelectedMonth", 0);
                HttpContext.Session.SetInt32("SelectedYear", 0);
            }

            base.OnActionExecuted(context);
        }
        
        [HttpGet]
        public async Task<IActionResult> CreateGoingOut(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User = await _userManager.FindByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            //Absence.EmpUserId = User.Id;
            ViewBag.firstName = User.FirstName;
            ViewBag.lastName = User.LastName;

            //GoingOut GoingOut = new GoingOut
            //{
            //    EmpUserId = User.Id,
            //    DateOfAbsence = DateTime.Now.Date,
            //    StartAbsence = DateTime.Now,
            //    EndAbsence = DateTime.Now,
            //    User = User
            //};

            Absence = new Absence
            {
                EmpUserId = User.Id,
                //User = User,
                DateOfAbsence = DateTime.Now.Date,
                EndDateOfAbsence = DateTime.Now.Date,
                StartAbsence = DateTime.Now,
                EndAbsence = DateTime.Now,
            };

            //_context.Add(GoingOut);
            //await _context.SaveChangesAsync();
                
                return View(Absence);
            
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoingOut ()
        {
            //absence.EmpUserId = Absence.EmpUserId;
            //GoingOut GoingOut = _context.GoingOut
            //    .Where(g => g.EmpUserId == Absence.EmpUserId && g.StartAbsence.Minute.Equals(Absence.StartAbsence.Minute))
            //    .FirstOrDefault();
            //if (GoingOut == null)
            //{
            //    return NotFound();
            //}
            if (ModelState.IsValid)
            {
                EmployeeUser user = await _userManager.FindByIdAsync(Absence.EmpUserId);

                if ((TypeOfDay)Absence.Type == TypeOfDay.EndWorkingDay)
                {
                    WorkingDay workDay = _context.WorkingDays
                        .Where(w => w.EmpUserId == Absence.EmpUserId && w.EndWork == new DateTime(01, 01, 01, 0, 0, 0))
                        .FirstOrDefault();

                    //EmployeeUser empUser = await _userManager.FindByIdAsync(Absence.EmpUserId);

                    workDay.EndWork = DateTime.Now;
                    user.IsWorking = false;

                    _context.Update(workDay);
                    _context.Update(user);
                    
                }

                else if ((TypeOfDay)Absence.Type == TypeOfDay.BusinessTrip 
                          || (TypeOfDay)Absence.Type == TypeOfDay.PaidLeave
                          || (TypeOfDay)Absence.Type == TypeOfDay.UnpaidLeave
                          || (TypeOfDay)Absence.Type == TypeOfDay.SickLeave)
                {
                    List<WorkingDay> workDays = _context.WorkingDays
                        .Where(w => w.EmpUserId == Absence.EmpUserId && w.Date == Absence.DateOfAbsence)
                        .ToList();
                    if (workDays.Count > 0)
                    {

                        foreach (WorkingDay workDay in workDays)
                        {
                            workDay.TypeOfDay = (TypeOfDay)Absence.Type;
                            _context.Update(workDay);
                        }
                    }
                    else
                    {
                        WorkingDay workDay = new WorkingDay
                        {
                            TypeOfDay = (TypeOfDay)Absence.Type,
                            Date = Absence.DateOfAbsence,
                            StartWork = DateTime.Now,
                            EndWork = DateTime.Now
                        };
                        _context.Add(workDay);
                    }
                }
                //if (Request.Form["IsPersonal"] == "true")
                //{
                //    goingOut.Reason = "Personal";

                //}
                //GoingOut.EmpUserId = User.Id;
                //GoingOut.User = User;
                GoingOut goingOut = new GoingOut
                {
                    EmpUserId = Absence.EmpUserId,
                    DateOfAbsence = Absence.DateOfAbsence,
                    StartAbsence = Absence.StartAbsence,
                    //EndAbsence = DateTime.Now,
                    Reason = Absence.Reason,
                    TypeOfDay = (TypeOfDay)Absence.Type,
                    
                    //HoursOfAbsence = Absence.EndAbsence.Subtract(Absence.StartAbsence),
                };

                if((TypeOfDay)Absence.Type != null)
                {
                    goingOut.TypeOfDay = (TypeOfDay)Absence.Type;
                    goingOut.NumberOfDays = Absence.NumberOfDays;
                }

                if(Absence.StartAbsence != Absence.EndAbsence)
                {
                    goingOut.EndAbsence = Absence.EndAbsence;
                }
                else
                {
                    goingOut.EndAbsence = DateTime.Now;
                }

                goingOut.HoursOfAbsence = goingOut.EndAbsence.Subtract(goingOut.StartAbsence);

                _context.Add(goingOut);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", user);
            }

            return View(Absence);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditGoingOut(string id)
        {
            if (id != null)
            {

                GoingOut = await _context.GoingOut
                    .Where(g => g.Id == id)
                    .FirstOrDefaultAsync();

                if(GoingOut == null)
                {
                    return NotFound();
                }
            }
            else
            {
                    return NotFound();
            }

            return View(GoingOut);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditGoingOut()
        {
            if (ModelState.IsValid)
            {
                //if ((TypeOfDay)GoingOut.TypeOfDay == TypeOfDay.EndWorkingDay)
                //{
                //    WorkingDay workDay = _context.WorkingDays
                //        .Where(w => w.EmpUserId == Absence.EmpUserId && w.EndWork == new DateTime(01, 01, 01, 0, 0, 0))
                //        .FirstOrDefault();

                //    EmployeeUser empUser = await _userManager.FindByIdAsync(Absence.EmpUserId);

                //    workDay.EndWork = DateTime.Now;
                //    empUser.IsWorking = false;

                //    _context.Update(workDay);
                //    _context.Update(empUser);

                //}
                EmployeeUser user = await _userManager.FindByIdAsync(GoingOut.EmpUserId);

                if (GoingOut.TypeOfDay != TypeOfDay.WorkingDay)
                {
                    List<WorkingDay> workDays = _context.WorkingDays
                        .Where(w => w.EmpUserId == GoingOut.EmpUserId && w.Date == GoingOut.DateOfAbsence)
                        .ToList();
                    if (workDays.Count > 0)
                    {

                        foreach (WorkingDay workDay in workDays)
                        {
                            workDay.TypeOfDay = GoingOut.TypeOfDay;
                            _context.Update(workDay);
                        }
                    }
                    else
                    {
                        WorkingDay workDay = new WorkingDay
                        {
                            TypeOfDay = GoingOut.TypeOfDay,
                            Date = GoingOut.DateOfAbsence,
                            StartWork = DateTime.Now,
                            EndWork = DateTime.Now
                        };
                        _context.Add(workDay);
                    }
                }

                GoingOut.HoursOfAbsence = GoingOut.EndAbsence.Subtract(GoingOut.StartAbsence);

                _context.Update(GoingOut);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", user);
            }
            return View(GoingOut);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            GoingOut goingOut = await _context.GoingOut
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync();
            if(goingOut == null)
            {
                return NotFound();
            }
            else
            {
                EmployeeUser user = await _userManager.FindByIdAsync(goingOut.EmpUserId);

                _context.GoingOut.Remove(goingOut);
                await _context.SaveChangesAsync();

                if (user == null)
                {
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index", user);
            }

        }
    }
}
