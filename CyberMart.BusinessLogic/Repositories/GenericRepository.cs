using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CyberMart.BusinessLogic.Interfaces;
using CyberMart.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CyberMart.BusinessLogic.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly CyberMartDbContext dbContext;
		private DbSet<T> dbSet;
		public GenericRepository(CyberMartDbContext _dbContext)
		{                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
			dbContext = _dbContext;
            dbSet = dbContext.Set<T>();
        }
		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? IncludeWord = null)
		{
			IQueryable<T> query = dbSet;
			if(predicate != null)
			{
				query = query.Where(predicate);
			}
			if(IncludeWord != null)
			{
				foreach(var word in IncludeWord.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(word);
				}
			}
			return query.ToList();
		}


		public T GetFirstElement(Expression<Func<T, bool>>? predicate = null, string? IncludeWord = null)
		{
			IQueryable<T> query = dbSet;
			if (predicate != null)
			{
				query = query.Where(predicate);
			}
			if (IncludeWord != null)
			{
				foreach (var word in IncludeWord.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(word);
				}
			}
			return query.SingleOrDefault();
		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

        public void RemoveRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}

		public void Update(T entity)
		{
			dbSet.Update(entity);
		}
	}
}
