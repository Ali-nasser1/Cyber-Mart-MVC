using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberMart.BusinessLogic.Interfaces;
using CyberMart.DataAccess.Contexts;
using CyberMart.DataAccess.Models;

namespace CyberMart.BusinessLogic.Repositories
{
	public class ProductRepository : GenericRepository<Product>, IProductRepository
	{
		public ProductRepository(CyberMartDbContext _dbContext) : base(_dbContext)
		{
		}
	}
}
