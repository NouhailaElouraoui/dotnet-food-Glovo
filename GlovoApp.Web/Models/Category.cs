using System.ComponentModel.DataAnnotations;

namespace Nouhaila.netProjet.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        
        // Navigation properties
        public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}
