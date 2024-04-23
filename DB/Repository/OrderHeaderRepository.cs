using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.DB.Data;

namespace EcommerceWebAppProject.DB.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly AppDbContext _dbContext;
		public OrderHeaderRepository(AppDbContext db) : base(db)
		{
			// Get db context from dependency injection to work with db
			_dbContext = db;
		}
		
		public void Update(OrderHeader orderHeader)
		{
			_dbContext.OrderHeader.Update(orderHeader);
		}
		
	}
}
