using EcommerceWebAppProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWebAppProject.DB.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { CatId = 1, CatName = "Book" },
                new Category { CatId = 2, CatName = "Clothes" },
                new Category { CatId = 3, CatName = "Food" }
            );

			modelBuilder.Entity<Product>().HasData(
				new Product { ProductId = 1, ProName = "A", Price = 10.5M, Qauntity=1, OriginCountry="KH", catId=1, ImageUrl=""},
				new Product { ProductId = 2, ProName = "B", Price = 14.5M, Qauntity = 2, OriginCountry = "US" , catId = 1, ImageUrl = ""},
				new Product { ProductId = 3, ProName = "C", Price = 5.5M, Qauntity = 3, OriginCountry = "KH" , catId = 2, ImageUrl = ""}
			);
		}

		
		 
	}
}
