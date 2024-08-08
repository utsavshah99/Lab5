using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;

namespace Lab5.Controllers
{
    public class SportClubController : Controller
    {
        private readonly SportsDbContext _context;

        public SportClubController(SportsDbContext context)
        {
            _context = context;
        }

        // GET: SportClub
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.SportClubs.ToListAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: SportClub/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                SportClub sportClub = await _context.SportClubs
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (sportClub == null)
                {
                    return NotFound();
                }

                return View(sportClub);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: SportClub/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SportClub/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] SportClub sportClub)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(sportClub);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to create sport club. Please try again later.");
                }
            }
            return View(sportClub);
        }

        // GET: SportClub/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                SportClub sportClub = await _context.SportClubs.FindAsync(id);
                if (sportClub == null)
                {
                    return NotFound();
                }
                return View(sportClub);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: SportClub/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] SportClub sportClub)
        {
            if (id != sportClub.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sportClub);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportClubExists(sportClub.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to update sport club. Please try again later.");
                }
            }
            return View(sportClub);
        }

        // GET: SportClub/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // Check if there are any news items associated with the sport club
                bool news = await _context.News.AnyAsync(n => n.SportClubId == id);

                if (news)
                {
                    TempData["ErrorMessage"] = "Cannot delete this SportClub because it has news items. Please delete the news items first.";
                    return RedirectToAction(nameof(Index));
                }

                SportClub sportClub = await _context.SportClubs
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (sportClub == null)
                {
                    return NotFound();
                }

                return View(sportClub);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: SportClub/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                SportClub sportClub = await _context.SportClubs.FindAsync(id);
                

                if (sportClub != null)
                {
                    _context.SportClubs.Remove(sportClub);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to delete sport club. Please try again later.");
                return View();
            }
        }

        private bool SportClubExists(string id)
        {
            return _context.SportClubs.Any(e => e.Id == id);
        }
    }
}
