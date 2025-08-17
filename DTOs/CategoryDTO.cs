using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.DTOs
{
    public class CategoryDTO
    {        
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }
    }
}