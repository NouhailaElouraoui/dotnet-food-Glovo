using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nouhaila.netProjet.Data;
using Nouhaila.netProjet.Models;
using Nouhaila.netProjet.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Nouhaila.netProjet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<Nouhaila.netProjet.Hubs.OrderHub> _hubContext;

        public AdminController(ApplicationDbContext context, IHubContext<Nouhaila.netProjet.Hubs.OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;
            var sevenDaysAgo = today.AddDays(-7);

            var orders = await _context.Orders.ToListAsync();
            var users = await _context.Users.ToListAsync();

            var stats = new AdminDashboardViewModel
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                TotalRevenue = orders.Sum(o => o.TotalPrice),
                TotalUsers = users.Count,
                OrdersToday = orders.Count(o => o.CreatedAt.Date == today),
                RevenueToday = orders.Where(o => o.CreatedAt.Date == today).Sum(o => o.TotalPrice),
                AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.TotalPrice) : 0,
                LoyaltyPointsDistributed = users.Sum(u => u.LoyaltyPoints),
                LatestOrders = orders.OrderByDescending(o => o.CreatedAt).Take(5).ToList()
            };

            // Prepare Chart Data (Last 7 Days)
            var chartData = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .OrderBy(d => d)
                .Select(date => new
                {
                    Date = date.ToString("MMM dd"),
                    Revenue = orders.Where(o => o.CreatedAt.Date == date).Sum(o => o.TotalPrice),
                    OrderCount = orders.Count(o => o.CreatedAt.Date == date)
                }).ToList();

            stats.DailyRevenueJson = JsonSerializer.Serialize(chartData.Select(d => d.Revenue));
            stats.DailyOrdersJson = JsonSerializer.Serialize(chartData.Select(d => d.OrderCount));
            ViewBag.ChartLabels = JsonSerializer.Serialize(chartData.Select(d => d.Date));

            return View(stats);
        }

        public async Task<IActionResult> Restaurants()
        {
            var restaurants = await _context.Restaurants.Include(r => r.Category).ToListAsync();
            return View(restaurants);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
                
                // SignalR Broadcast
                await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("OrderStatusUpdated", orderId, status.ToString());
            }
            return RedirectToAction(nameof(Index));
        }

        // Create Restaurant
        public async Task<IActionResult> CreateRestaurant()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRestaurant(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Restaurants));
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(restaurant);
        }

        // Edit Restaurant
        public async Task<IActionResult> EditRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null) return NotFound();
            
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(restaurant);
        }

        [HttpPost]
        public async Task<IActionResult> EditRestaurant(int id, Restaurant restaurant)
        {
            if (id != restaurant.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Restaurants));
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(restaurant);
        }

        public async Task<IActionResult> ManageMenu(int restaurantId)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Products)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
            
            if (restaurant == null) return NotFound();
            
            return View(restaurant);
        }

        // Product CRUD
        public IActionResult CreateProduct(int restaurantId)
        {
            ViewBag.RestaurantId = restaurantId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageMenu), new { restaurantId = product.RestaurantId });
            }
            ViewBag.RestaurantId = product.RestaurantId;
            return View(product);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageMenu), new { restaurantId = product.RestaurantId });
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                int restaurantId = product.RestaurantId;
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageMenu), new { restaurantId = restaurantId });
            }
            return RedirectToAction(nameof(Restaurants));
        }

        // Promotions CRUD
        public async Task<IActionResult> Promotions()
        {
            var promos = await _context.Promotions.ToListAsync();
            return View(promos);
        }

        public IActionResult CreatePromotion() => View();

        [HttpPost]
        public async Task<IActionResult> CreatePromotion(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Promotions));
            }
            return View(promotion);
        }

        public async Task<IActionResult> EditPromotion(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo == null) return NotFound();
            return View(promo);
        }

        [HttpPost]
        public async Task<IActionResult> EditPromotion(int id, Promotion promotion)
        {
            if (id != promotion.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Promotions));
            }
            return View(promotion);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo != null)
            {
                _context.Promotions.Remove(promo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Promotions));
        }

        // Categories CRUD
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult CreateCategory() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (id != category.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Categories));
        }
    }
}
