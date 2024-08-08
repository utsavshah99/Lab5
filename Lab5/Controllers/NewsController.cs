 using Lab5.Data;
using Lab5.Models;
using Lab5.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Controllers
{
    public class NewsController : Controller
    {
        private readonly SportsDbContext _context;

        public NewsController(SportsDbContext context)
        {
            _context = context;
        }

        // GET: News/Index/{id}
        public async Task<IActionResult> Index(string id)
        {
            try
            {
                // Get a list of news associated with the given SportClubId
                List<News> news = await _context.News
                    .Where(n => n.SportClubId == id)
                    .ToListAsync();

                // Get the SportClub entity that matches the given id
                SportClub sportClub = await _context.SportClubs
                    .FirstOrDefaultAsync(sc => sc.Id == id);

                if (sportClub == null)
                {
                    return NotFound();
                }

                NewsViewModel viewModel = new NewsViewModel
                {
                    SportClub = sportClub,
                    News = news
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: News/Create/{id}
        public IActionResult Create(string id)
        {
            FileInputViewModel viewModel = new FileInputViewModel
            {
                SportClubId = id
            };
            return View(viewModel);
        }

        // POST: News/Create/{id}
        [HttpPost]
        public async Task<IActionResult> Create(FileInputViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a file has been uploaded and if its length is greater than 0
                    if (viewModel.File != null && viewModel.File.Length > 0)
                    {
                        // Get the file name from the uploaded file
                        string fileName = System.IO.Path.GetFileName(viewModel.File.FileName);

                        // Define the path where the file will be saved
                        string filePath = System.IO.Path.Combine("wwwroot/images/news", fileName);

                        // Save the uploaded file to the defined path
                        using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                        {
                            await viewModel.File.CopyToAsync(stream);
                        }

                        // Create a new News object with the provided data
                        News news = new News
                        {
                            SportClubId = viewModel.SportClubId,
                            ImageUrl = $"/images/news/{fileName}",
                            Title = viewModel.Title,
                            Description = viewModel.Description
                        };

                        _context.News.Add(news);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index), new { id = viewModel.SportClubId });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to create news. Please try again later.");
                }
            }
            return View(viewModel);
        }

        // GET: News/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                News news = await _context.News
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (news == null)
                {
                    return NotFound();
                }

                return View(news);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: News/Delete/{id}
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                News news = await _context.News.FindAsync(id);
                if (news != null)
                {
                    _context.News.Remove(news);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index), new { id = news.SportClubId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to delete news. Please try again later.");
                return View();
            }
        }
    }
}
