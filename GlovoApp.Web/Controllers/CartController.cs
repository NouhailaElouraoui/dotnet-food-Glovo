using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nouhaila.netProjet.Data;
using Nouhaila.netProjet.Models;
using Nouhaila.netProjet.ViewModels;
using System.Security.Claims;

namespace Nouhaila.netProjet.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "CartId";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCartId()
        {
            if (HttpContext.Session.GetString(CartSessionKey) == null)
            {
                if (!string.IsNullOrWhiteSpace(User.Identity?.Name))
                {
                    HttpContext.Session.SetString(CartSessionKey, User.Identity.Name);
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Session.SetString(CartSessionKey, tempCartId.ToString());
                }
            }
            return HttpContext.Session.GetString(CartSessionKey)!;
        }

        public async Task<IActionResult> Index()
        {
            var cartId = GetCartId();
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.SessionId == cartId || (User.Identity != null && User.Identity.IsAuthenticated && c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)))
                .ToListAsync();

            var viewModel = new ShoppingCartViewModel { Items = cartItems };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var cartId = GetCartId();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == productId && (c.SessionId == cartId || (userId != null && c.UserId == userId)));

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    SessionId = cartId,
                    UserId = userId
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
