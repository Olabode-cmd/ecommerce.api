using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}