using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;
using Zapp.Models;
using Zapp.Models.ViewModels;

namespace Zapp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
              return _context.Customer != null ? 
                          View(await _context.Customer.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Customer'  is null.");
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            var model = new CustomerViewModel { Customer = new Customer() };
            model.AllTasks = _context.TaskItem.ToList();
            return View(model);
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.AllTasks = _context.TaskItem.ToList();
                    return View(model);
                }

                var customer = model.Customer;
                var customerTasks = model.CustomerTasks;

                // Process new customer
                _context.Add(customer);
                _context.SaveChanges();

                var theCustomer = _context.Customer
                    .OrderBy(e => e.Id)
                    .Last();
                if (theCustomer == null)
                {
                    throw new Exception("Something went wrong while retrieving the customer from the database.");
                }
                customer.Id = theCustomer.Id;

                // Process customer tasks
                foreach (var customerTask in customerTasks)
                {
                    customerTask.CustomerId = customer.Id;

                    var theTask = _context.TaskItem
                    .Where(e => e.Name == customerTask.Task.Name)
                    .FirstOrDefault();
                    if (theTask == null)
                    {
                        TaskItem newTaskItem = new TaskItem() { Name = customerTask.Task.Name };
                        _context.TaskItem.Add(newTaskItem);
                        _context.SaveChanges();

                        theTask = _context.TaskItem.OrderBy(e => e.Id).Last();
                        if (theTask == null)
                        {
                            throw new Exception("Something went wrong while retrieving the task from the database.");
                        }
                    }
                    customerTask.TaskId = theTask.Id;

                    if (!customerTask.IsDeleted)
                    {
                        CustomerTask newCustomerTask = new CustomerTask()
                        {
                            CustomerId = customerTask.CustomerId,
                            TaskId = customerTask.TaskId,
                            AdditionalInfo = customerTask.AdditionalInfo
                        };
                        _context.CustomerTask.Add(newCustomerTask);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan.");
                model.AllTasks = _context.TaskItem.ToList();
                return View(model);
            }
        }

        // GET: Customer/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Customer == null)
            {
                return NotFound();
            }

            var customer = _context.Customer
                .Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            var customerTasks = _context.CustomerTask
                .Where(e => e.CustomerId == customer.Id)
                .ToList()
                .ToArray();

            var model = new CustomerViewModel {
                Customer = customer,
            };
            if (customerTasks != null && customerTasks.Length != 0)
            {
                model.CustomerTasks = customerTasks;
            }
            model.AllTasks = _context.TaskItem.ToList();
            return View(model);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CustomerViewModel model)
        {
            try
            {
                for (int i = 0; i < model.CustomerTasks.Count(); i++)
                {
                    ModelState.Remove($"CustomerTasks[{i}].Customer");
                    ModelState.Remove($"AppointmentTasks[{i}].Task.Name");
                }

                if (!ModelState.IsValid)
                {
                    model.AllTasks = _context.TaskItem.ToList();
                    return View(model);
                }

                var customer = model.Customer;
                var customerTasks = model.CustomerTasks;

                // Process customer
                _context.Update(customer);
                _context.SaveChanges();

                // Process customer tasks
                var savedCustomerTasks = _context.CustomerTask
                    .Where(e => e.CustomerId == customer.Id)
                    .ToList()
                    .ToArray();
                    
                foreach (var customerTask in customerTasks)
                {
                    customerTask.CustomerId = customer.Id;

                    var theTask = _context.TaskItem
                        .Where(e => e.Name == customerTask.Task.Name)
                        .FirstOrDefault();
                    if (theTask == null)
                    {
                        TaskItem newTaskItem = new TaskItem() { Name = customerTask.Task.Name };
                        _context.TaskItem.Add(newTaskItem);
                        _context.SaveChanges();

                        theTask = _context.TaskItem.OrderBy(e => e.Id).Last();
                        if (theTask == null)
                        {
                            throw new Exception("Something went wrong while retrieving the task from the database.");
                        }
                    }
                    customerTask.TaskId = theTask.Id;

                    bool foundMatch = false;
                    if (savedCustomerTasks != null)
                    {
                        foreach (var savedTask in savedCustomerTasks)
                        {
                            if (customerTask.TaskId == savedTask.TaskId)
                            {
                                if (customerTask.IsDeleted)
                                {
                                    _context.CustomerTask.Remove(savedTask);
                                }
                                else
                                {
                                    savedTask.AdditionalInfo = customerTask.AdditionalInfo;
                                }
                                foundMatch = true;
                                break;
                            }
                        }
                        if (!foundMatch)
                        {
                            if (!customerTask.IsDeleted)
                            {
                                CustomerTask newCustomerTask = new CustomerTask()
                                {
                                    CustomerId = customerTask.CustomerId,
                                    TaskId = customerTask.TaskId,
                                    AdditionalInfo = customerTask.AdditionalInfo
                                };
                                _context.CustomerTask.Add(newCustomerTask);
                            }
                        }
                    }
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(model.Customer.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("ModelOnly", "Er is iets fout gegaan.");
                    model.AllTasks = _context.TaskItem.ToList();
                    return View(model);
                }
            }
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customer == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customer'  is null.");
            }
            var customer = await _context.Customer.FindAsync(id);
            if (customer != null)
            {
                _context.Customer.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
