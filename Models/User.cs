using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models
{
    public class User
    {
        // Primary Key
        public Guid Id { get; set; } = Guid.NewGuid();

        // Username for display / login
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        // Email will be used for login as well
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Securely stored password (hashed, never plain text)
        [Required]
        public string PasswordHash { get; set; }
    }
}
