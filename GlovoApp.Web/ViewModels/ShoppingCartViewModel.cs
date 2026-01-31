using Nouhaila.netProjet.Models;

namespace Nouhaila.netProjet.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => i.Quantity * (i.Product?.Price ?? 0));
    }
}
