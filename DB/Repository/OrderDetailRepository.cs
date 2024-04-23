using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.DB.Data;

namespace EcommerceWebAppProject.DB.Repository
{
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
	{
		private readonly AppDbContext _dbContext;
		public OrderDetailRepository(AppDbContext db) : base(db)
		{
			// Get db context from dependency injection to work with db
			_dbContext = db;
		}
		
		public void Update(OrderDetail orderDetail)
		{
			_dbContext.OrderDetail.Update(orderDetail);
		}
		
	}
}
