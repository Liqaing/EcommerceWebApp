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

		public T Get(Expression<Func<T, bool>> filter)
		{
			IQueryable<T> query = this.dbSet;
			query = query.Where(filter);
			return query.FirstOrDefault();
		}

		public IEnumerable<T> GetAll()
		{
			IQueryable<T> query = dbSet;
			return query.ToList();
		}

		
	}
}
