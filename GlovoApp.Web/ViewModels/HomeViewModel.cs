using Nouhaila.netProjet.Models;

namespace Nouhaila.netProjet.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Restaurant> PopularRestaurants { get; set; } = new List<Restaurant>();
        public IEnumerable<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}
