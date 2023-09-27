using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;
using Zapp.Models;

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
            viewModel = FillAppointmentViewModel(viewModel);

            return View(viewModel);
        }

        private AppointmentViewModel FillAppointmentViewModel(AppointmentViewModel viewModel)
        {
            viewModel.Appointment.Scheduled = DateTime.Today;
            viewModel.AllCustomers = _context.Customer.ToList();
            viewModel.AllEmployees = _context.Users.ToList();
            viewModel.AllTasks = _context.TaskItem.ToList();
            return viewModel;
        }

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
            var customerId = viewModel.Appointment.CustomerId;
            var employeeId = viewModel.Appointment.EmployeeId;
            var appointmentTasks = viewModel.AppointmentTasks;
            bool isEmpty = true;
            foreach (var appTask in appointmentTasks)
            {
                if (appTask.Task.Name != null)
                {
                    isEmpty = false;
                    break;
                }
            }
            
            if (customerId == null || employeeId == null || isEmpty)
            {
                viewModel = FillAppointmentViewModel(viewModel);
                return View(viewModel);
            }

            try
            {
                if (_context.Appointment
                    .Where(e => e.EmployeeId == viewModel.Appointment.EmployeeId)
                    .Any(e => e.Scheduled == viewModel.Appointment.Scheduled))
                {
                    ModelState.AddModelError("Appointment.Scheduled", "Deze medewerker is niet beschikbaar op de gekozen tijdstip.");
                    viewModel = FillAppointmentViewModel(viewModel);
                    return View(viewModel);
                }

                Appointment appointment = viewModel.Appointment;

                var customer = _context.Customer.Find(appointment.CustomerId);
                if (customer != null)
                {
                    appointment.Customer = customer;
                }

                var employee = _context.Users.Find(appointment.EmployeeId);
                if (employee != null)
                {
                    appointment.Employee = employee;
                }
                _context.Appointment.Add(appointment);
                _context.SaveChanges();

                var savedAppointment = _context.Appointment.OrderBy(e => e.Id).Last();

                if (savedAppointment != null)
                {
                    foreach (var appointmentTask in viewModel.AppointmentTasks)
                    {
                        appointmentTask.AppointmentId = savedAppointment.Id;

                        appointmentTask.Appointment = savedAppointment;

                        var task = _context.TaskItem
                            .Where(e => e.Name == appointmentTask.Task.Name)
                            .FirstOrDefault();

                        if (task != null)
                        {
                            appointmentTask.TaskId = task.Id;
                            appointmentTask.Task = task;
                        }
                        else
                        {
                            TaskItem newTask = new TaskItem()
                            {
                                Name = appointmentTask.Task.Name
                            };
                            _context.TaskItem.Add(newTask);

                            var savedTask = _context.TaskItem.OrderBy(e => e.Id).Last();
                            if (savedTask != null)
                            {
                                appointmentTask.TaskId = savedTask.Id;
                                appointmentTask.Task = savedTask;
                            }
                        }
                        _context.AppointmentTask.Add(appointmentTask);
                        savedAppointment.AppointmentTasks.Add(appointmentTask);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { Id = viewModel.Appointment.Id });
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan");
                viewModel = FillAppointmentViewModel(viewModel);
                return View(viewModel);
            }
        }

        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Set<Customer>(), "Id", "Id", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", appointment.EmployeeId);
            return View(appointment);
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,EmployeeId,Scheduled,CheckedIn,CheckedOut")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Set<Customer>(), "Id", "Id", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", appointment.EmployeeId);
            return View(appointment);
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
