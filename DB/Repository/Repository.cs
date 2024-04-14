
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.DB.Repository;
using Microsoft.EntityFrameworkCore;
using EcommerceWebAppProject.DB.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EcommerceWebAppProject.DB.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly AppDbContext _dbContext;

		// Represent class that this obj create on
		internal DbSet<T> dbSet;

		public Repository(AppDbContext db)
		{
			// Get db context from dependency injection to work with db
			_dbContext = db;
			this.dbSet = _dbContext.Set<T>();
		}

		public void Add(T entity)
		{
			this.dbSet.Add(entity);
		}

		public void Delete(T entity)
		{
			dbSet.Remove(entity);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool track = false)
		{
			IQueryable<T> query;
			if (track)
			{
				query = this.dbSet;
			}
			else
			{
				query = this.dbSet.AsNoTracking();
			}

			query = query.Where(filter);

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (string property in includeProperties.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			return query.FirstOrDefault();
		}

		/// <summary>Gets all.</summary>
		/// <param name="includeProperties">The include properties. list of string represent related entity to include in query</param>
		/// <returns>
		///   <br />
		/// </returns>
		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;

			if (filter != null)
			{
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach(string property in includeProperties.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			return query.ToList();
		}

		
	}
}
