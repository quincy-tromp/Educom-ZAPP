using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var applicationDbContext = _context.Appointment.Include(a => a.Customer).Include(a => a.Employee);
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
            //ViewData["CustomerId"] = new SelectList(_context.Set<Customer>(), "Id", "Id");
            //ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id");

            AppointmentViewModel viewModel = new AppointmentViewModel() { Appointment = new Appointment() };
            viewModel = BuAppointment.FillAppointmentViewModel(_context, viewModel);
            return View(viewModel);
        }
        // OLD CODE
        //private AppointmentViewModel FillAppointmentViewModel(AppointmentViewModel viewModel)
        //{
        //    viewModel.Appointment.Scheduled = DateTime.Today;
        //    viewModel.AllCustomers = _context.Customer.ToList();
        //    viewModel.AllEmployees = _context.Users.ToList();
        //    viewModel.AllTasks = _context.TaskItem.ToList();
        //    return viewModel;
        //}

        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppointmentViewModel viewModel)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(appointment);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["CustomerId"] = new SelectList(_context.Set<Customer>(), "Id", "Id", appointment.CustomerId);
            //ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", appointment.EmployeeId);
            //return View(appointment);

            try
            {
                if (BuAppointment.IsAppointmentEmpty(viewModel))
                {
                    ModelState.AddModelError("ModelOnly", "Deze afspraak is leeg.");
                    viewModel = BuAppointment.FillAppointmentViewModel(_context, viewModel);
                    return View(viewModel);
                }
                if (BuAppointment.IsEmployeeUnavailable(_context, viewModel))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "Deze medewerker is niet beschikbaar op de gekozen tijdstip.");
                    viewModel = BuAppointment.FillAppointmentViewModel(_context, viewModel);
                    return View(viewModel);
                }
                if (BuAppointment.IsAppointmentTasksEmpty(viewModel.AppointmentTasks))
                {
                    ModelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
                    viewModel = BuAppointment.FillAppointmentViewModel(_context, viewModel);
                    return View(viewModel);
                }


                Appointment newAppointment = viewModel.Appointment;
                var customer = _context.Customer.Find(newAppointment.CustomerId);
                if (customer != null)
                {
                    newAppointment.Customer = customer;
                }
                var employee = _context.Users.Find(newAppointment.EmployeeId);
                if (employee != null) {
                    newAppointment.Employee = employee;
                }
                _context.Appointment.Add(newAppointment);
                _context.SaveChanges();


                var appointment = _context.Appointment.OrderBy(e => e.Id).Last();
                if (appointment != null)
                {
                    foreach (var appointmentTask in viewModel.AppointmentTasks)
                    {
                        appointmentTask.AppointmentId = appointment.Id;
                        appointmentTask.Appointment = appointment;

                        var taskItem = _context.TaskItem
                            .Where(e => e.Name == appointmentTask.Task.Name)
                            .FirstOrDefault();
                        if (taskItem != null)
                        {
                            appointmentTask.TaskId = taskItem.Id;
                            appointmentTask.Task = taskItem;
                        }
                        else
                        {
                            TaskItem newTaskItem = new TaskItem() { Name = appointmentTask.Task.Name };
                            _context.TaskItem.Add(newTaskItem);
                            _context.SaveChanges();

                            var task = _context.TaskItem.OrderBy(e => e.Id).Last();
                            if (task != null)
                            {
                                appointmentTask.TaskId = task.Id;
                                appointmentTask.Task = task;
                            }
                        }
                        _context.AppointmentTask.Add(appointmentTask);
                        appointment.AppointmentTasks.Add(appointmentTask);
                        _context.SaveChanges();
                    }

                    var customerTasks = appointment.Customer.CustomerTasks;
                    if (customerTasks != null)
                    {
                        foreach (var customerTask in customerTasks)
                        {
                            var appointmentTask = new AppointmentTask()
                            {
                                AppointmentId = appointment.Id,
                                Appointment = appointment,
                                TaskId = customerTask.TaskId,
                                Task = customerTask.Task,
                                AdditionalInfo = customerTask.AdditionalInfo
                            };
                            _context.AppointmentTask.Add(appointmentTask);
                            appointment.AppointmentTasks.Add(appointmentTask);
                            _context.SaveChanges();
                        }
                    }
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { Id = viewModel.Appointment.Id });
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan");
                viewModel = BuAppointment.FillAppointmentViewModel(_context, viewModel);
                return View(viewModel);
            }
        }

        // GET: Appointment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = _context.Appointment.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }
            var scheduled = appointment.Scheduled;

            AppointmentViewModel viewModel = new AppointmentViewModel()
            {
                Appointment = appointment
            };
            viewModel = BuAppointment.FillAppointmentViewModel(_context, viewModel);
            viewModel.Appointment.Scheduled = scheduled;

            var appointmentTasks = _context.AppointmentTask
                .Where(e => e.AppointmentId == appointment.Id)
                .ToList();
            if (appointmentTasks == null)
            {
                return NotFound();
            }

            viewModel.AppointmentTasks = appointmentTasks.ToArray();

            var customer = _context.Customer.Find(appointment.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }
            viewModel.Appointment.Customer = customer;

            var customerTasks = _context.CustomerTask
                .Where(e => e.CustomerId == customer.Id)
                .ToList();
            if (customerTasks == null)
            {
                return NotFound();
            }
            viewModel.CustomerTasks = customerTasks.ToArray();

            var employee = _context.Users.Find(appointment.EmployeeId);
            if (employee == null)
            {
                return NotFound();
            }

            viewModel.Appointment.Employee = employee;

            //var customer = await _context.Customer.FindAsync(appointment.CustomerId);
            //if (customer == null)
            //{
            //    return NotFound();
            //}

            //var customerTasks = _context.CustomerTask
            //    .Where(e => e.CustomerId == customer.Id)
            //    .ToList();
            
            //viewModel.CustomerTasks = customerTasks.ToArray();

            //ViewData["CustomerId"] = new SelectList(_context.Set<Customer>(), "Id", "Id", appointment.CustomerId);
            //ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", appointment.EmployeeId);
            return View(viewModel);
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentViewModel viewModel)
        {
            //viewModel.AppointmentTasks = BuAppointment.dropEmptyAppointmentTasks(viewModel.AppointmentTasks);
            //if (id != appointment.Id)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(appointment);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!AppointmentExists(appointment.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["CustomerId"] = new SelectList(_context.Set<Customer>(), "Id", "Id", appointment.CustomerId);
            //ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", appointment.EmployeeId);
            return View(viewModel);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Appointment == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Appointment'  is null.");
            }
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointment.Remove(appointment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
          return (_context.Appointment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
