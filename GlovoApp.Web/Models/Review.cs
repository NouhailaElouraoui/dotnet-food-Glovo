using System.ComponentModel.DataAnnotations;

namespace Nouhaila.netProjet.Models
{
    public class Review
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        
        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
