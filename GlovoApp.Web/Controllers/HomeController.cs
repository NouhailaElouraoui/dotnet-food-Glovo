using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nouhaila.netProjet.Data;
using Nouhaila.netProjet.Models;
using Nouhaila.netProjet.ViewModels;

namespace Nouhaila.netProjet.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        ViewData["CurrentFilter"] = searchString;

        var restaurantsQuery = _context.Restaurants
                                    .Include(r => r.Category)
                                    .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            restaurantsQuery = restaurantsQuery.Where(s => s.Name.Contains(searchString) 
                                                        || (s.Category != null && s.Category.Name.Contains(searchString)));
        }

        var model = new HomeViewModel
        {
            Categories = await _context.Categories.ToListAsync(),
            PopularRestaurants = await restaurantsQuery
                                    .OrderByDescending(r => r.Rating)
                                    .Take(8)
                                    .ToListAsync(),
            Promotions = await _context.Promotions
                                    .Where(p => p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow)
                                    .ToListAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
