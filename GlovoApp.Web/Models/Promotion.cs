using System.ComponentModel.DataAnnotations;

namespace Nouhaila.netProjet.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        
        public double DiscountPercentage { get; set; } // e.g., 0.20 for 20%
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    }
}
