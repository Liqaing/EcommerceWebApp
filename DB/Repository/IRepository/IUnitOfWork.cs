using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.DB.Repository.IRepository
{
	/// <summary>Interface for class that create and manage other dbAccess class, 
	/// so we won't have to create individual class in order to access db</summary>
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
		IProductRepository Product { get; }
		IShoppingCartRepository ShoppingCart { get; }
		IAppUserRepository AppUser { get; }
		IOrderDetailRepository OrderDetail { get; }
		IOrderHeaderRepository OrderHeader { get; }
		void Save();
	}
}
