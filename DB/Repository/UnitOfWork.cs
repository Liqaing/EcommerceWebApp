using EcommerceWebAppProject.DB.Data;
using EcommerceWebAppProject.DB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.DB.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _dbContext;
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }
		public UnitOfWork(AppDbContext db)
		{
			// Get db context from dependency injection to work with db
			_dbContext = db;
			Category = new CategoryRepository(db);
			Product = new ProductRepository(db);
		}

		public void Save()
		{
			_dbContext.SaveChanges();
		}
	}
}
