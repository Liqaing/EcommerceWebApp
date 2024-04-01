using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.DB.Data;

namespace EcommerceWebAppProject.DB.Repository
{
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private readonly AppDbContext _dbContext;
		public CategoryRepository(AppDbContext db) : base(db)
		{
			// Get db context from dependency injection to work with db
			_dbContext = db;
		}
		
		public void Update(Category cat)
		{
			_dbContext.Category.Update(cat);
		}
		
	}
}
