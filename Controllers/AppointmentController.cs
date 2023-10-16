using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;
using Zapp.Models;
using Zapp.Models.Business;
using Zapp.Models.BusinessLogic;

namespace Zapp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private AppointmentControllerHelper _helper;

        public AppointmentController(ApplicationDbContext context, AppointmentControllerHelper helper)
        {
            _context = context;
            _helper = helper;
        }

// GET: Appointment
        public IActionResult Index()
        {
            var applicationDbContext = _context.Appointment
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .OrderBy(a => a.Scheduled);
            return View(nameof(Index), applicationDbContext.ToList());
        }


// GET: Appointment/Create
        public IActionResult Create()
        {
            var model = AppointmentHelper.CreateNewViewModel();
            model = AppointmentHelper.InitializeViewModel(_context, model, true);

            return View(nameof(Create), model);
        }


// POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentViewModel model)
        {
            try
            {   // Bring model to a valid state
                ModelState.Clear();
                // Filter appointment tasks
                model.AppointmentTasks = AppointmentHelper.removeEmptyAppointmentTasks(model.AppointmentTasks);
                model.AppointmentTasks = AppointmentHelper.removeDuplicateAppointmentTasks(model.AppointmentTasks);

                // Validate data 
                if (AppointmentValidator.IsIntIdZero(model.Appointment.CustomerId))
                {
                    ModelState.AddModelError("Appointment.CustomerId", "Selecteer een klant.");
                }
                if (AppointmentValidator.IsIdNull(model.Appointment.EmployeeId))
                {
                    ModelState.AddModelError("Appointment.EmployeeId", "Selecteer een medewerker.");
                }
                if (!AppointmentValidator.IsValidDate(model.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen datum is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsValidTime(model.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen tijd is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsEmployeeAvailable(_context, model.Appointment.EmployeeId, model.Appointment.Scheduled, null))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "Dit medewerker is niet beschikbaar op de gekozen tijdstip.");
                }
                if (AppointmentValidator.IsEmptyAppointmentTasks(model.AppointmentTasks))
                {
                    ModelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
                    model.AppointmentTasks = new AppointmentTask[] { new AppointmentTask() };
                }
                if (!ModelState.IsValid)
                {
                    model = AppointmentHelper.InitializeViewModel(_context, model, false);
                    return View(nameof(Create), model);
                }

                // Process appointment
                _context.Appointment.Add(model.Appointment);
                _context.SaveChanges();

                var theAppointment = _context.Appointment
                    .Include(e => e.Customer)
                    .Include(e => e.Customer.CustomerTasks)
                    .OrderBy(e => e.Id).Last();
                if (theAppointment == null)
                {
                    throw new Exception("Something went wrong while retrieving the appointment from the database");
                }

                // Process customer tasks
                var customerTasks = theAppointment.Customer.CustomerTasks;
                if (customerTasks != null)
                {
                    foreach (var customerTask in customerTasks)
                    {
                        var appointmentTask = new AppointmentTask()
                        {
                            AppointmentId = theAppointment.Id,
                            TaskId = customerTask.TaskId,
                            AdditionalInfo = customerTask.AdditionalInfo
                        };
                        _context.AppointmentTask.Add(appointmentTask);
                    }
                    _context.SaveChanges();
                }

                // Process appointment tasks
                var appointmentTasks = model.AppointmentTasks;
                for (int i = 0; i < appointmentTasks.Count(); i++)
                {
                    var theTask = _context.TaskItem
                        .Where(e => e.Name == appointmentTasks[i].Task.Name)
                        .FirstOrDefault();
                    if (theTask == null)
                    {
                        TaskItem newTaskItem = new TaskItem() { Name = appointmentTasks[i].Task.Name };
                        _context.TaskItem.Add(newTaskItem);
                        _context.SaveChanges();

                        theTask = _context.TaskItem.OrderBy(e => e.Id).Last();
                        if (theTask == null)
                        {
                            throw new Exception("Something went wrong while retrieving the task from the database.");
                        }
                    }

                    appointmentTasks[i].TaskId = theTask.Id;
                    appointmentTasks[i].AppointmentId = theAppointment.Id;


                    var savedAppointmentTasks = _context.AppointmentTask
                    .Where(e => e.AppointmentId == theAppointment.Id)
                    .ToList()
                    .ToArray();

                    bool foundMatch = false;

                    if (savedAppointmentTasks != null && savedAppointmentTasks.Length > 0)
                    {
                        foreach (var savedTask in savedAppointmentTasks)
                        {
                            if (appointmentTasks[i].TaskId == savedTask.TaskId)
                            {
                                if (!appointmentTasks[i].IsDeleted)
                                {
                                    savedTask.IsDone = appointmentTasks[i].IsDone;
                                    savedTask.AdditionalInfo = appointmentTasks[i].AdditionalInfo;
                                    _context.AppointmentTask.Update(savedTask);
                                } 
                                foundMatch = true;
                                break;
                            }
                        }
                    }

                    if (!foundMatch)
                    {
                        if (!appointmentTasks[i].IsDeleted)
                        {
                            AppointmentTask newAppointmentTask = new AppointmentTask()
                            {
                                AppointmentId = appointmentTasks[i].AppointmentId,
                                TaskId = appointmentTasks[i].TaskId,
                                AdditionalInfo = appointmentTasks[i].AdditionalInfo,
                                IsDone = appointmentTasks[i].IsDone
                            };
                            _context.AppointmentTask.Add(newAppointmentTask);
                        }
                    }
                    _context.SaveChanges();
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan");
                model = AppointmentHelper.InitializeViewModel(_context, model, true);
                return View(nameof(Create), model);
            }
        }


// GET: Appointment/Edit/5
        public IActionResult Edit(int id)
        {
            if (_context.Appointment == null)
            {
                return NotFound();
            }
            var model = AppointmentHelper.CreateNewViewModel();
            model = AppointmentHelper.InitializeViewModel(_context, model, false);
            model = AppointmentHelper.AddAppointmentToViewModel(_context, model, id);

            return View(nameof(Edit), model);
        }


// POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppointmentViewModel model)
        {
            try
            {
                // Filter appointment tasks
                model.AppointmentTasks = AppointmentHelper.removeEmptyAppointmentTasks(model.AppointmentTasks);
                model.AppointmentTasks = AppointmentHelper.removeDuplicateAppointmentTasks(model.AppointmentTasks);

                // Bring model to a valid state 
                ModelState.Clear();

                // Validate data
                if (AppointmentValidator.IsIntIdZero(model.Appointment.CustomerId))
                {
                    ModelState.AddModelError("Appointment.CustomerId", "Selecteer een klant.");
                }
                if (AppointmentValidator.IsIdNull(model.Appointment.EmployeeId))
                {
                    ModelState.AddModelError("Appointment.EmployeeId", "Selecteer een medewerker.");
                }
                if (!AppointmentValidator.IsValidDate(model.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen datum is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsValidTime(model.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen tijd is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsEmployeeAvailable(_context, model.Appointment.EmployeeId, model.Appointment.Scheduled, model.Appointment.Id))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "Dit medewerker is niet beschikbaar op de gekozen tijdstip.");
                }
                if (AppointmentValidator.IsEmptyAppointmentTasks(model.AppointmentTasks))
                {
                    ModelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
                }
                if (!ModelState.IsValid)
                {
                    model = AppointmentHelper.InitializeViewModel(_context, model, false);
                    model = AppointmentHelper.AddAppointmentToViewModel(_context, model, model.Appointment.Id);
                    return View(nameof(Edit), model);
                }

                // Process appointment
                var theAppointment = _context.Appointment.Find(model.Appointment.Id);
                if (theAppointment == null)
                {
                    throw new Exception("Something went wrong while retrieving the appointment from the database.");
                }
                theAppointment.Scheduled = model.Appointment.Scheduled;
                theAppointment.EmployeeId = model.Appointment.EmployeeId;

                // Get saved appointment tasks
                var savedAppointmentTasks = _context.AppointmentTask
                    .Where(e => e.AppointmentId == theAppointment.Id)
                    .ToList()
                    .ToArray();
                if (savedAppointmentTasks == null)
                {
                    throw new Exception("Something went wrong while retrieving the scheduled appointment tasks from the database.");
                }

                // Process appointment tasks
                var appointmentTasks = model.AppointmentTasks;
                for (int i = 0; i < appointmentTasks.Count(); i++) 
                {
                    var theTask = _context.TaskItem
                        .Where(e => e.Name == appointmentTasks[i].Task.Name)
                        .FirstOrDefault();
                    if (theTask == null)
                    {
                        TaskItem newTaskItem = new TaskItem() { Name = appointmentTasks[i].Task.Name };
                        _context.TaskItem.Add(newTaskItem);
                        _context.SaveChanges();

                        theTask = _context.TaskItem.OrderBy(e => e.Id).Last();
                        if (theTask == null)
                        {
                            throw new Exception("Something went wrong while retrieving the task from the database.");
                        }
                    }

                    appointmentTasks[i].TaskId = theTask.Id;
                    appointmentTasks[i].AppointmentId = theAppointment.Id;

                    bool foundMatch = false;
                    foreach (var savedTask in savedAppointmentTasks)
                    {
                        if (appointmentTasks[i].TaskId == savedTask.TaskId)
                        {
                            if (appointmentTasks[i].IsDeleted)
                            {
                                _context.AppointmentTask.Remove(savedTask);
                                _context.SaveChanges();
                            }
                            else
                            {
                                savedTask.IsDone = appointmentTasks[i].IsDone;
                                savedTask.AdditionalInfo = appointmentTasks[i].AdditionalInfo;
                            }
                            foundMatch = true;
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        if (!appointmentTasks[i].IsDeleted)
                        {
                            AppointmentTask newAppointmentTask = new AppointmentTask()
                            {
                                AppointmentId = appointmentTasks[i].AppointmentId,
                                TaskId = appointmentTasks[i].TaskId,
                                AdditionalInfo = appointmentTasks[i].AdditionalInfo,
                                IsDone = appointmentTasks[i].IsDone
                            };
                            _context.AppointmentTask.Add(newAppointmentTask);
                        }
                    }
                    _context.SaveChanges();
                }
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan");
                model = AppointmentHelper.InitializeViewModel(_context, model, false);
                model = AppointmentHelper.AddAppointmentToViewModel(_context, model, model.Appointment.Id);
                return View(nameof(Edit), model);
            }

        }


// POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(AppointmentViewModel model)
        {
            try
            {
                if (_context.Appointment == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Appointment'  is null.");
                }
                var appointment =  _context.Appointment.Find(model.Appointment.Id);

                if (appointment == null)
                {
                    throw new Exception("Something went wrong while retrieving appointment from the database.");
                }

                var appointmentTasks = _context.AppointmentTask
                    .Where(a => a.AppointmentId == appointment.Id)
                    .ToList();
                foreach (var task in appointmentTasks)
                {
                    _context.AppointmentTask.Remove(task);
                }
                _context.Appointment.Remove(appointment);

                 _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool AppointmentExists(int id)
        {
          return (_context.Appointment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
