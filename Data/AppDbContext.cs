using Microsoft.EntityFrameworkCore;
using EcommerceApi.Models;

namespace EcommerceApi.Data
{
    // AppDbContext inherits from EF Core's DbContext
    // It manages database connections and maps C# models to database tables
    public class AppDbContext : DbContext
    {
        // Constructor passes options (like which DB to use) to the base DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }

        // DbSet<User> means "create a Users table in the database"
        // Add Users table
        public DbSet<User> Users { get; set; }
        
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
