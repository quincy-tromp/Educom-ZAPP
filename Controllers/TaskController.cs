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
        public async Task<IActionResult> Index()
        {
              return _context.CareTask != null ? 
                          View(await _context.CareTask.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.CareTask'  is null.");
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CareTask == null)
            {
                return NotFound();
            }

            var careTask = await _context.CareTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (careTask == null)
            {
                return NotFound();
            }

            return View(careTask);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CareTask careTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(careTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(careTask);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CareTask == null)
            {
                return NotFound();
            }

            var careTask = await _context.CareTask.FindAsync(id);
            if (careTask == null)
            {
                return NotFound();
            }
            return View(careTask);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CareTask careTask)
        {
            if (id != careTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(careTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CareTaskExists(careTask.Id))
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
            return View(careTask);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CareTask == null)
            {
                return NotFound();
            }

            var careTask = await _context.CareTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (careTask == null)
            {
                return NotFound();
            }

            return View(careTask);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CareTask == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CareTask'  is null.");
            }
            var careTask = await _context.CareTask.FindAsync(id);
            if (careTask != null)
            {
                _context.CareTask.Remove(careTask);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CareTaskExists(int id)
        {
          return (_context.CareTask?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
