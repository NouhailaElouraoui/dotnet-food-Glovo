using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nouhaila.netProjet.Data;
using Nouhaila.netProjet.Models;
using System.Security.Claims;

namespace Nouhaila.netProjet.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string address, PaymentMethod paymentMethod, string? promoCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            decimal subtotal = cartItems.Sum(i => i.Quantity * (i.Product?.Price ?? 0));
            decimal discount = 0;

            // Simple Promo Code Logic
            if (!string.IsNullOrEmpty(promoCode))
            {
                var promo = await _context.Promotions.FirstOrDefaultAsync(p => p.Code == promoCode && p.EndDate >= DateTime.UtcNow);
                if (promo != null)
                {
                    discount = (subtotal * (decimal)promo.DiscountPercentage) / 100;
                }
            }

            var order = new Order
            {
                UserId = userId!,
                DeliveryAddress = address,
                TotalPrice = subtotal - discount,
                DiscountAmount = discount,
                PromoCode = promoCode,
                PaymentMethod = paymentMethod,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product?.Price ?? 0
                }).ToList()
            };

            // Reward loyalty points (1 point per dollar spent)
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LoyaltyPoints += (int)order.TotalPrice;
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Tracking", new { id = order.Id });
        }

        public async Task<IActionResult> Tracking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }
    }
}
