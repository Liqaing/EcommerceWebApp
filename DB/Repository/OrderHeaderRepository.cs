using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.DB.Data;
using Microsoft.IdentityModel.Tokens;

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

		public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null)
		{
			OrderHeader? orderHeader = _dbContext.OrderHeader.FirstOrDefault(
					o => o.OrderHeaderId == id);
		
			if (orderHeader != null)
			{
				orderHeader.OrderStatus = OrderStatus;
				
				if (PaymentStatus != null)
				{
					orderHeader.PaymentStatus = PaymentStatus;
				}

				_dbContext.OrderHeader.Update(orderHeader);
			}
		}

		public void UpdateStripePayment(int id, string? SessionId, string? PaymentIntentId)
		{
			OrderHeader? orderHeader = _dbContext.OrderHeader.FirstOrDefault(
					o => o.OrderHeaderId == id);
			if (orderHeader != null)
			{
				if (!string.IsNullOrEmpty(orderHeader.SessionId))
				{
					orderHeader.SessionId = SessionId;
				}

				if (!string.IsNullOrEmpty(orderHeader.PaymentIntentId))
				{
					orderHeader.PaymentIntentId = PaymentIntentId;
					orderHeader.PaymentDate = DateTime.Now;
				}
			}
			
		}
	}
}
