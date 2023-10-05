using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;
using Zapp.Models;
using Zapp.Models.Business;

namespace Zapp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

// GET: Appointment
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointment
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .OrderBy(a => a.Scheduled);
            return View(await applicationDbContext.ToListAsync());
        }


// GET: Appointment/Create
        public ActionResult Create()
        {
            var viewModel = AppointmentHelper.CreateNewViewModel();
            viewModel = AppointmentHelper.FillViewModel(_context, viewModel, true);
            return View(viewModel);
        }


// POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppointmentViewModel viewModel)
        {
            try
            {   // Bring model to a valid state
                ModelState.Remove("Appointment.Customer");
                ModelState.Remove("Appointment.Employee");
                for (int i = 0; i < viewModel.AppointmentTasks.Count(); i++)
                {
                    ModelState.Remove($"AppointmentTasks[{i}].Appointment");
                }
                // Filter appointment tasks
                viewModel.AppointmentTasks = AppointmentHelper.removeEmptyAppointmentTasks(viewModel.AppointmentTasks);
                viewModel.AppointmentTasks = AppointmentHelper.removeDuplicateAppointmentTasks(viewModel.AppointmentTasks);

                // Validate data 
                if (viewModel.Appointment.CustomerId == 0)
                {
                    ModelState.AddModelError("Appointment.CustomerId", "Kies een klant.");
                }
                if (viewModel.Appointment.EmployeeId == null)
                {
                    ModelState.AddModelError("Appointment.EmployeeId", "Kies een medewerker.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, true);
                    return View(viewModel);
                }
                if (!AppointmentValidator.IsValidDate(viewModel.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen datum is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsValidTime(viewModel.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen tijd is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsEmployeeAvailable(_context, viewModel.Appointment.EmployeeId, viewModel.Appointment.Scheduled, null))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "Dit medewerker is niet beschikbaar op de gekozen tijdstip.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, true);
                    return View(viewModel);
                }
                if (viewModel.AppointmentTasks.Count() == 0 || viewModel.AppointmentTasks == null)
                {
                    ModelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    return View(viewModel);
                }

                // Process appointment
                _context.Appointment.Add(viewModel.Appointment);
                _context.SaveChanges();

                var theAppointment = _context.Appointment
                    .Include(e => e.Customer)
                    .Include(e => e.Customer.CustomerTasks)
                    .OrderBy(e => e.Id).Last();
                if (theAppointment == null)
                {
                    throw new Exception("Something went wrong while retrieving the appointment from the database");
                }

                // Process appointment tasks
                var appointmentTasks = viewModel.AppointmentTasks;
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

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan");
                viewModel = AppointmentHelper.FillViewModel(_context, viewModel, true);
                return View(viewModel);
            }
        }


// GET: Appointment/Edit/5
        public ActionResult Edit(int id)
        {
            if (_context.Appointment == null)
            {
                return NotFound();
            }

            var viewModel = AppointmentHelper.CreateViewModelWithAppointment(_context, id);
            viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);

            return View(viewModel);
        }


// POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppointmentViewModel viewModel)
        {
            try
            {   // Bring model to a valid state 
                ModelState.Remove("Appointment.Customer");
                ModelState.Remove("Appointment.Employee");
                for (int i = 0; i < viewModel.AppointmentTasks.Count(); i++)
                {
                    ModelState.Remove($"AppointmentTasks[{i}].Appointment");
                    ModelState.Remove($"AppointmentTasks[{i}].Task");
                }
                // Filter appointment tasks
                viewModel.AppointmentTasks = AppointmentHelper.removeEmptyAppointmentTasks(viewModel.AppointmentTasks);
                viewModel.AppointmentTasks = AppointmentHelper.removeDuplicateAppointmentTasks(viewModel.AppointmentTasks);

                // Validate data
                if (viewModel.Appointment.CustomerId == 0)
                {
                    ModelState.AddModelError("Appointment.CustomerId", "Kies een klant.");
                }
                if (viewModel.Appointment.EmployeeId == null)
                {
                    ModelState.AddModelError("Appointment.EmployeeId", "Kies een medewerker.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.CreateViewModelWithAppointment(_context, viewModel.Appointment.Id);
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    return View(viewModel);
                }
                if (!AppointmentValidator.IsValidDate(viewModel.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen datum is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsValidTime(viewModel.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen tijd is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsEmployeeAvailable(_context, viewModel.Appointment.EmployeeId, viewModel.Appointment.Scheduled, viewModel.Appointment.Id))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "Dit medewerker is niet beschikbaar op de gekozen tijdstip.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.CreateViewModelWithAppointment(_context, viewModel.Appointment.Id);
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    return View(viewModel);
                }
                if (viewModel.AppointmentTasks.Count() == 0 || viewModel.AppointmentTasks == null)
                {
                    ModelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.CreateViewModelWithAppointment(_context, viewModel.Appointment.Id);
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    return View(viewModel);
                }

                // Process appointment
                var theAppointment = _context.Appointment.Find(viewModel.Appointment.Id);
                if (theAppointment == null)
                {
                    throw new Exception("Something went wrong while retrieving the appointment from the database.");
                }
                theAppointment.Scheduled = viewModel.Appointment.Scheduled;
                theAppointment.EmployeeId = viewModel.Appointment.EmployeeId;

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
                var appointmentTasks = viewModel.AppointmentTasks;
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
                }
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan");
                viewModel = AppointmentHelper.CreateViewModelWithAppointment(_context, viewModel.Appointment.Id);
                viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                return View(viewModel);
            }

        }


// POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(AppointmentViewModel viewModel)
        {
            try
            {
                if (_context.Appointment == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Appointment'  is null.");
                }
                var appointment =  _context.Appointment.Find(viewModel.Appointment.Id);

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
