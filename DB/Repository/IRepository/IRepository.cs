﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.DB.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
		// Get all value from db
		IEnumerable<T> GetAll();
		T Get(Expression<Func<T, bool>> filter);
		void Add(T entity);
		void Delete(T entity);
	}
}
