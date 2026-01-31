using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nouhaila.netProjet.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPopular { get; set; }
        public ProductType Type { get; set; } = ProductType.Individual;

        // Foreign Keys
        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
    }
}
