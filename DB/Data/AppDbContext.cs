using EcommerceWebAppProject.Models;
using Microsoft.EntityFrameworkCore;
using Models;

namespace EcommerceWebAppProject.DB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        // Create category table in db
        public DbSet<Category> Category { get; set; }
		public DbSet<Product> Product { get; set; }

		// Seeding data on when model created
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CatId = 1, CatName = "Book" },
                new Category { CatId = 2, CatName = "Clothes" },
                new Category { CatId = 3, CatName = "Food" }
            );

			modelBuilder.Entity<Product>().HasData(
				new Product { ProductId = 1, ProName = "A", Price = 10.5M, OriginCountry="KH" },
				new Product { ProductId = 2, ProName = "B", Price = 14.5M, OriginCountry = "US" },
				new Product { ProductId = 3, ProName = "C", Price = 5.5M, OriginCountry = "KH" }
			);
		}

		
		 
	}
}
