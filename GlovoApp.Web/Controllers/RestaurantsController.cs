using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nouhaila.netProjet.Data;
using Nouhaila.netProjet.Models;
using Nouhaila.netProjet.ViewModels;

namespace Nouhaila.netProjet.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Products)
                .FirstOrDefaultAsync(m => m.Id == id); // Simplified query first

            if (restaurant == null)
            {
                return NotFound();
            }

            var model = new RestaurantDetailsViewModel
            {
                Restaurant = restaurant,
                MenuProducts = restaurant.Products,
                Categories = restaurant.Products.Select(p => p.Type.ToString()).Distinct().ToList()
            };

            return View(model);
        }
    }
}
