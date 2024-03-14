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
            _dbContext.Product.Update(pro);
        }
        
    }
}
