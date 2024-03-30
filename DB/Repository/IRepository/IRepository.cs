using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.DB.Repository.IRepository
{
	/// <summary>Generic interface for repository class which have common method to interact with db</summary>
	/// <typeparam name="T"></typeparam>
	public interface IRepository<T> where T : class
	{
        // Get all value from db

        /// <summary>Gets all.</summary>
        /// <param name="includeProperties">The include properties. list of string represent related entity to include in query</param>
        IEnumerable<T> GetAll(string? includeProperties = null);

		T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool track =  false);
		
		void Add(T entity);
		
		void Delete(T entity);
	}
}
