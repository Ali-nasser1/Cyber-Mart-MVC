using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberMart.BusinessLogic.Interfaces;
using CyberMart.DataAccess.Contexts;

namespace CyberMart.BusinessLogic.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly CyberMartDbContext dbContext;

		public ICategoryRepository Category {  get; private set; }

		public IProductRepository Product { get; private set; }

		public IShoppingCartRepository ShoppingCart { get; private set; }

		public IOrderDetailsRepository OrderDetails { get; private set; }

		public IOrderHeaderRepository OrderHeader { get; private set; }

		public IApplicationUserRepository ApplicationUser { get; private set; }

		public UnitOfWork(CyberMartDbContext _dbContext)
		{
			dbContext = _dbContext;
			Category = new CategoryRepository(_dbContext);
			Product = new ProductRepository(_dbContext);
			ShoppingCart = new ShoppingCartRepository(_dbContext);
			OrderDetails = new OrderDetailsRepository(_dbContext);
			OrderHeader = new OrderHeaderRepository(_dbContext);
			ApplicationUser = new ApplicationUserRepository(_dbContext);
		}
		public int Complete()
		{
			return dbContext.SaveChanges();
		}

		public void Dispose()
		{
			dbContext.Dispose();
		}
	}
}
