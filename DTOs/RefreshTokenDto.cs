using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}