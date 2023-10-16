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
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Task
        public IActionResult Index()
        {
              return _context.TaskItem != null ? 
                          View(nameof(Index), _context.TaskItem.ToList()) :
                          Problem("Entity set 'ApplicationDbContext.TaskItem'  is null.");
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            return View(nameof(Create));
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name")] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskItem);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ModelState.Remove("Name");
            ModelState.AddModelError("Name", "Voer een taak in.");
            return View(nameof(Create), taskItem);
        }

        // GET: Task/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.TaskItem == null)
            {
                return NotFound();
            }

            var taskItem = _context.TaskItem
                .FirstOrDefault(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(nameof(Delete), taskItem);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_context.TaskItem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TaskItem'  is null.");
            }
            var taskItem = _context.TaskItem.Find(id);
            if (taskItem != null)
            {
                _context.TaskItem.Remove(taskItem);
            }
            
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
