using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.DB.Data;

namespace EcommerceWebAppProject.DB.Repository
{
	public class AppUserRepository : Repository<AppUser>, IAppUserRepository
	{
		private readonly AppDbContext _dbContext;
		public AppUserRepository(AppDbContext db) : base(db)
		{
			// Get db context from dependency injection to work with db
			_dbContext = db;
		}
	}
}
