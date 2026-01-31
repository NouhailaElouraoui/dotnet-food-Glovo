using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nouhaila.netProjet.Data;
using Nouhaila.netProjet.Models;
using System.Security.Claims;

namespace Nouhaila.netProjet.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int orderId, int rating, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null || order.Status != OrderStatus.Delivered)
            {
                return BadRequest("Order not found or not delivered.");
            }

            // Get the first restaurant from the order items
            var firstItem = order.OrderItems.FirstOrDefault();
            if (firstItem?.Product == null) return BadRequest();
            
            var restaurantId = firstItem.Product.RestaurantId;

            var review = new Review
            {
                UserId = userId!,
                RestaurantId = restaurantId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Update average rating (simplified)
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant != null)
            {
                var allReviews = await _context.Reviews.Where(r => r.RestaurantId == restaurantId).ToListAsync();
                restaurant.Rating = Math.Round(allReviews.Average(r => r.Rating), 1);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyOrders", "Orders");
        }
    }
}
