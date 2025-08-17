using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public List<string> Tags { get; set; } = new();

        public IFormFile Image { get; set; }
    }

    public class UpdateProductDto
    {
        [MaxLength(200)]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public Guid? CategoryId { get; set; }

        public List<string> Tags { get; set; }

        public IFormFile Image { get; set; }
    }
}
