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

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointment/Create
        public IActionResult Create()
        {
            AppointmentViewModel viewModel = new AppointmentViewModel() { Appointment = new Appointment() };
            viewModel = AppointmentHelper.FillViewModel(_context, viewModel, true);
            return View(viewModel);
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppointmentViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Appointment.Customer");
                ModelState.Remove("Appointment.Employee");
                for (int i = 0; i < viewModel.AppointmentTasks.Count(); i++)
                {
                    ModelState.Remove($"AppointmentTasks[{i}].Appointment");
                }

                viewModel.AppointmentTasks = AppointmentHelper.removeEmptyAppointmentTasks(viewModel.AppointmentTasks);
                viewModel.AppointmentTasks = AppointmentHelper.removeDuplicateAppointmentTasks(viewModel.AppointmentTasks);

                // Validate Appointment.Customer 
                if (viewModel.Appointment.CustomerId == 0)
                {
                    ModelState.AddModelError("Appointment.CustomerId", "Kies een klant.");
                }
                // Validate Appointment.Employee
                if (viewModel.Appointment.EmployeeId == null)
                {
                    ModelState.AddModelError("Appointment.EmployeeId", "Kies een medewerker.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, true);
                    return View(viewModel);
                }
                //Validate Appointment.Scheduled
                if (!AppointmentValidator.IsValidDateTime(viewModel.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen tijd en/of datum is niet beschikbaar.");
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
                // Validate AppointmentTasks
                if (viewModel.AppointmentTasks.Count() == 0 || viewModel.AppointmentTasks == null)
                {
                    ModelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    return View(viewModel);
                }

                // Process Appointment
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

                // Process AppointmentTasks
                foreach (var appointmentTask in viewModel.AppointmentTasks)
                {
                    appointmentTask.AppointmentId = theAppointment.Id;

                    var taskItem = _context.TaskItem
                        .Where(e => e.Name == appointmentTask.Task.Name)
                        .FirstOrDefault();

                    if (taskItem == null)
                    {
                        TaskItem newTaskItem = new TaskItem() { Name = appointmentTask.Task.Name };
                        _context.TaskItem.Add(newTaskItem);
                        _context.SaveChanges();
                        var task = _context.TaskItem.OrderBy(e => e.Id).Last();

                        if (task == null)
                        {
                            throw new Exception("Something went wrong while retrieving the task from the database.");
                        }

                        appointmentTask.TaskId = task.Id;
                    }
                    else
                    {
                        appointmentTask.TaskId = taskItem.Id;
                    }
                    _context.AppointmentTask.Add(appointmentTask);
                    _context.SaveChanges();
                }

                // Process customerTasks
                var customerTasks = theAppointment.Customer.CustomerTasks;
                //var customerTasks = _context.CustomerTask.Where(e => e.CustomerId == theAppointment.CustomerId);

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

        private AppointmentViewModel getAppointmentForEdit(int id)
        {
            var appointment = _context.Appointment.Find(id);
            if (appointment == null)
            {
                throw new Exception("Something went wrong while retrieving the appointment from the database.");
            }

            AppointmentViewModel viewModel = new AppointmentViewModel()
            {
                Appointment = appointment
            };
            viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);

            var customer = _context.Customer.Find(appointment.CustomerId);
            if (customer == null)
            {
                throw new Exception("Something went wrong while retrieving the customer from the database.");
            }
            viewModel.Appointment.Customer = customer;

            var employee = _context.Users.Find(appointment.EmployeeId);
            if (employee == null)
            {
                throw new Exception("Something went wrong while retrieving the employee from the database.");
            }
            viewModel.Appointment.Employee = employee;

            var appointmentTasks = _context.AppointmentTask
                .Where(e => e.AppointmentId == appointment.Id)
                .ToList();
            if (appointmentTasks == null)
            {
                throw new Exception("Something went wrong while retrieving the appointmentTasks from the database.");
            }
            viewModel.AppointmentTasks = appointmentTasks.ToArray();

            var customerTasks = _context.CustomerTask
                .Where(e => e.CustomerId == customer.Id)
                .ToList();
            if (customerTasks != null)
            {
                viewModel.CustomerTasks = customerTasks.ToArray();
            }

            return viewModel;
        }

        // GET: Appointment/Edit/5
        public ActionResult Edit(int id)
        {
            if (_context.Appointment == null)
            {
                return NotFound();
            }

            AppointmentViewModel viewModel = getAppointmentForEdit(id);

            return View(viewModel);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AppointmentViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Appointment.Customer");
                ModelState.Remove("Appointment.Employee");
                for (int i = 0; i < viewModel.AppointmentTasks.Count(); i++)
                {
                    ModelState.Remove($"AppointmentTasks[{i}].Appointment");
                }

                viewModel.AppointmentTasks = AppointmentHelper.removeEmptyAppointmentTasks(viewModel.AppointmentTasks);
                viewModel.AppointmentTasks = AppointmentHelper.removeDuplicateAppointmentTasks(viewModel.AppointmentTasks);

                // Validate Appointment.Customer 
                if (viewModel.Appointment.CustomerId == 0)
                {
                    ModelState.AddModelError("Appointment.CustomerId", "Kies een klant.");
                }
                // Validate Appointment.Employee
                if (viewModel.Appointment.EmployeeId == null)
                {
                    ModelState.AddModelError("Appointment.EmployeeId", "Kies een medewerker.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    viewModel = getAppointmentForEdit(viewModel.Appointment.Id);
                    return View(viewModel);
                }
                //Validate Appointment.Scheduled
                if (!AppointmentValidator.IsValidDateTime(viewModel.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "De gekozen tijd en/of datum is niet beschikbaar.");
                }
                if (!AppointmentValidator.IsEmployeeAvailable(_context, viewModel.Appointment.EmployeeId, viewModel.Appointment.Scheduled, viewModel.Appointment.Id))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "Dit medewerker is niet beschikbaar op de gekozen tijdstip.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    viewModel = getAppointmentForEdit(viewModel.Appointment.Id);
                    return View(viewModel);
                }
                // Validate AppointmentTasks
                if (viewModel.AppointmentTasks.Count() == 0 || viewModel.AppointmentTasks == null)
                {
                    ModelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
                }
                if (!ModelState.IsValid)
                {
                    viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                    viewModel = getAppointmentForEdit(viewModel.Appointment.Id);
                    return View(viewModel);
                }




                // EDIT
                // Get saved appointment
                var appointment = _context.Appointment.Find(viewModel.Appointment.Id);
                if (appointment == null)
                {
                    throw new Exception("Something went wrong while retrieving the appointment from the database.");
                }
                // Update saved appointment
                appointment.Scheduled = viewModel.Appointment.Scheduled;
                appointment.EmployeeId = viewModel.Appointment.EmployeeId;

                // Get appointment tasks already scheduled
                var savedAppointmentTasks = _context.AppointmentTask
                    .Where(e => e.AppointmentId == appointment.Id)
                    .ToList()
                    .ToArray();
                if (savedAppointmentTasks == null)
                {
                    throw new Exception("Something went wrong while retrieving the scheduled appointment tasks from the database.");
                }
                // Process each appointment task
                foreach (var appointmentTask in viewModel.AppointmentTasks)
                {
                    // Remove appointment task from Db if condition is true
                    if (appointmentTask.IsDeleted)
                    {
                        _context.AppointmentTask.Remove(appointmentTask);
                    }
                    else
                    {
                        // Get the task
                         var theTask = _context.TaskItem
                            .Where(e => e.Name == appointmentTask.Task.Name)
                            .FirstOrDefault();
                        if (theTask == null)
                        {
                            // Create new task if the task doesn't already exist
                            TaskItem newTaskItem = new TaskItem() { Name = appointmentTask.Task.Name };
                            _context.TaskItem.Add(newTaskItem);
                            _context.SaveChanges();
                            // Get the newly created task
                            theTask = _context.TaskItem.OrderBy(e => e.Id).Last();
                            if (theTask == null)
                            {
                                throw new Exception("Something went wrong while retrieving the task from the database.");
                            }
                        }
                        // Update saved appointment tasks
                        bool foundMatch = false;
                        foreach (var savedTask in savedAppointmentTasks)
                        {
                            if (theTask.Id == savedTask.TaskId)
                            {
                                savedTask.IsDone = appointmentTask.IsDone;
                                savedTask.AdditionalInfo = appointmentTask.AdditionalInfo;
                                appointment.AppointmentTasks.Add(savedTask);
                                foundMatch = true;
                                break;
                            }
                        }
                        // Add new appointment task
                        if (!foundMatch)
                        {
                            AppointmentTask newAppointmentTask = new AppointmentTask()
                            {
                                AppointmentId = appointment.Id,
                                TaskId = theTask.Id,
                                AdditionalInfo = appointmentTask.AdditionalInfo,
                                IsDone = appointmentTask.IsDone
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
                viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                viewModel = getAppointmentForEdit(viewModel.Appointment.Id);
                return View(viewModel);
            }

        }

        // GET: Appointment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
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
                //viewModel = AppointmentHelper.FillViewModel(_context, viewModel, false);
                //viewModel = getAppointmentData(viewModel.Appointment.Id);
                return RedirectToAction(nameof(Index));
            }
        }

        private bool AppointmentExists(int id)
        {
          return (_context.Appointment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
