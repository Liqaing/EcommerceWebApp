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

        // Seeding data on when model created
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CatId = 1, CatName = "Book" },
                new Category { CatId = 2, CatName = "Clothes" },
                new Category { CatId = 3, CatName = "Food" }
            );
        }
    }
}
