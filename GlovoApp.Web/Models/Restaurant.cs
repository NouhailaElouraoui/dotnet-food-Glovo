using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nouhaila.netProjet.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        // Delivery info
        public string DeliveryTimeRange { get; set; } = "20-30 min"; // Display string e.g., "20-30 min"
        public int PreparationTimeMinutes { get; set; } = 15; // Internal calc if needed
        public decimal DeliveryFee { get; set; } = 0m;
        
        public double Rating { get; set; } = 4.5;
        public int ReviewCount { get; set; } = 0;

        // Foreign keys
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
