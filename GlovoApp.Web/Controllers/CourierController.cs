using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nouhaila.netProjet.Data;
using Nouhaila.netProjet.Models;
using Microsoft.AspNetCore.SignalR;

namespace Nouhaila.netProjet.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<Nouhaila.netProjet.Hubs.OrderHub> _hubContext;

        public CourierController(ApplicationDbContext context, IHubContext<Nouhaila.netProjet.Hubs.OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var courierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Orders currently assigned to this courier
            var myOrders = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.CourierId == courierId && o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled)
                .ToListAsync();

            // Orders waiting for a courier (Marketplace)
            var availableOrders = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.CourierId == null && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing))
                .ToListAsync();

            ViewBag.AvailableOrders = availableOrders;
            return View(myOrders);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var courierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null && order.CourierId == null)
            {
                order.CourierId = courierId;
                order.Status = OrderStatus.OnTheWay;
                await _context.SaveChangesAsync();

                // SignalR Broadcast
                await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("OrderStatusUpdated", orderId, order.Status.ToString());
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeliverOrder(int orderId)
        {
            var courierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null && order.CourierId == courierId)
            {
                order.Status = OrderStatus.Delivered;
                await _context.SaveChangesAsync();

                // SignalR Broadcast
                await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("OrderStatusUpdated", orderId, order.Status.ToString());
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
