using EcommerceWebAppProject.DB.Data;
using EcommerceWebAppProject.DB.Repository;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.DB.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private readonly AppDbContext _dbContext;
        public ProductRepository(AppDbContext db) : base(db)
        {
            // Get db context from dependency injection to work with db
            _dbContext = db;
        }

        public void Update(Product pro)
        {
            Product? product = _dbContext.Product.FirstOrDefault(
                pro => pro.ProductId == pro.ProductId);
        
            if (product != null)
            {
                product.ProName = pro.ProName;
                product.Price = pro.Price;
                product.Description = pro.Description;
                product.Qauntity = pro.Qauntity;
                product.OriginCountry = pro.OriginCountry;
                product.CatId = pro.CatId;
                
                if (pro.ImageUrl != null)
                {
                    product.ImageUrl = pro.ImageUrl;
                }
            }
        }
        
    }
}
