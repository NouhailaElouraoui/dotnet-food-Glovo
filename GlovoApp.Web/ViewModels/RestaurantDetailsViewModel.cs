using Nouhaila.netProjet.Models;

namespace Nouhaila.netProjet.ViewModels
{
    public class RestaurantDetailsViewModel
    {
        public Restaurant Restaurant { get; set; } = new();
        public IEnumerable<Product> MenuProducts { get; set; } = new List<Product>();
        public IEnumerable<string> Categories { get; set; } = new List<string>();
    }
}
