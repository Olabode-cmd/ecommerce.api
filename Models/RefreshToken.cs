using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Token { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        public User User { get; set; }
        
        public DateTime ExpiresAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRevoked { get; set; } = false;
    }
}