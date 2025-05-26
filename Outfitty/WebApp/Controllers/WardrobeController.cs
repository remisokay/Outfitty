using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APP.DAL.EF;
using Domain;

namespace WebApp.Controllers
{
    public class WardrobeController : Controller
    {
        private readonly AppDbContext _context;

        public WardrobeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Wardrobe
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Wardrobes.Include(w => w.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Wardrobe/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardrobe = await _context.Wardrobes
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wardrobe == null)
            {
                return NotFound();
            }

            return View(wardrobe);
        }

        // GET: Wardrobe/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username");
            return View();
        }

        // POST: Wardrobe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,UserId,Id,CreatedBy,CreatedAt,ChangedBy,ChangedAt,SysNotes")] Wardrobe wardrobe)
        {
            if (ModelState.IsValid)
            {
                wardrobe.Id = Guid.NewGuid();
                _context.Add(wardrobe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", wardrobe.UserId);
            return View(wardrobe);
        }

        // GET: Wardrobe/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardrobe = await _context.Wardrobes.FindAsync(id);
            if (wardrobe == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", wardrobe.UserId);
            return View(wardrobe);
        }

        // POST: Wardrobe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,UserId,Id,CreatedBy,CreatedAt,ChangedBy,ChangedAt,SysNotes")] Wardrobe wardrobe)
        {
            if (id != wardrobe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wardrobe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardrobeExists(wardrobe.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", wardrobe.UserId);
            return View(wardrobe);
        }

        // GET: Wardrobe/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardrobe = await _context.Wardrobes
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wardrobe == null)
            {
                return NotFound();
            }

            return View(wardrobe);
        }

        // POST: Wardrobe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var wardrobe = await _context.Wardrobes.FindAsync(id);
            if (wardrobe != null)
            {
                _context.Wardrobes.Remove(wardrobe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardrobeExists(Guid id)
        {
            return _context.Wardrobes.Any(e => e.Id == id);
        }
    }
}
