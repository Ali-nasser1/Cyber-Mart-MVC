﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CyberMart.BusinessLogic.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? IncludeWord = null);
		T GetFirstElement(Expression<Func<T, bool>>? predicate = null, string? IncludeWord = null);
		void Update(T entity);
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);
	}
}
