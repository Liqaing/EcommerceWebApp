using EcommerceWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        // Create category table in db
        public DbSet<Category> Category { get; set; }
    }
}
