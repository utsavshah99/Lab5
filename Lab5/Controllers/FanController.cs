using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Lab5.Models.ViewModels;

namespace Lab5.Controllers
{
    public class FanController : Controller
    {
        private readonly SportsDbContext _context;

        public FanController(SportsDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var fans = await _context.Fans.ToListAsync();
                return View(fans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: Fans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var fan = await _context.Fans
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (fan == null)
                {
                    return NotFound();
                }

                return View(fan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(fan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to create fan. Please try again later.");
                }
            }
            return View(fan);
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var fan = await _context.Fans.FindAsync(id);
                if (fan == null)
                {
                    return NotFound();
                }
                return View(fan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: Fans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (id != fan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FanExists(fan.Id))
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
                    ModelState.AddModelError("", "Unable to update fan. Please try again later.");
                }
            }
            return View(fan);
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                Fan fan = await _context.Fans
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (fan == null)
                {
                    return NotFound();
                }

                return View(fan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Fan fan = await _context.Fans.FindAsync(id);
                if (fan != null)
                {
                    _context.Fans.Remove(fan);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to delete fan. Please try again later.");
                return View();
            }
        }

        private bool FanExists(int id)
        {
            return _context.Fans.Any(e => e.Id == id);
        }

        // GET: Fans/EditSubscriptions/id
        public async Task<IActionResult> EditSubscriptions(int id)
        {
            try
            {
                // Get the fan from the database, including their subscriptions and the related sport clubs
                Fan fan = await _context.Fans
                    .Include(f => f.Subscriptions)
                    .ThenInclude(s => s.SportClub)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (fan == null)
                {
                    return NotFound();
                }

                List<SportClub> sportClubs = await _context.SportClubs.ToListAsync();

                // Create the view model to pass to the view
                FanSubscriptionViewModel viewModel = new FanSubscriptionViewModel
                {
                    Fan = fan,
                    Subscriptions = sportClubs.Select(sc => new SportClubSubscriptionViewModel
                    {
                        SportClubId = sc.Id,
                        Title = sc.Title,
                        IsMember = fan.Subscriptions.Any(s => s.SportClubId == sc.Id)
                    }).OrderByDescending(s => s.IsMember).ThenBy(s => s.Title).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: Fans/UpdateSubscriptions/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSubscriptions(int fanId, string[] subscriptions)
        {
            try
            {
                // Get the fan from the database, including their subscriptions
                Fan fan = await _context.Fans
                    .Include(f => f.Subscriptions)
                    .FirstOrDefaultAsync(f => f.Id == fanId);

                if (fan == null)
                {
                    return NotFound();
                }

                List<string> subscriptionList = subscriptions.ToList();

                // Get the current subscriptions as a list
                List<Subscription> currentSubscriptions = fan.Subscriptions.ToList();
                List<string> currentSubscriptionIds = currentSubscriptions.Select(s => s.SportClubId).ToList();

                List<Subscription> toRemove = currentSubscriptions
                    .Where(s => !subscriptionList.Contains(s.SportClubId))
                    .ToList();

                // Remove subscriptions
                _context.Subscriptions.RemoveRange(toRemove);

                List<string> toAdd = subscriptionList
                    .Where(id => !currentSubscriptionIds.Contains(id))
                    .ToList();

                // Add new subscriptions
                foreach (var sportClubId in toAdd)
                {
                    _context.Subscriptions.Add(new Subscription
                    {
                        FanId = fanId,
                        SportClubId = sportClubId
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { id = fanId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to update subscriptions. Please try again later.");
                return View();
            }
        }
    }
}
