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
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly CyberMartDbContext dbContext;

		public OrderHeaderRepository(CyberMartDbContext _dbContext) : base(_dbContext)
		{
			dbContext = _dbContext;
		}

		public void UpdateOrderStatus(int Id, string OrderStatus, string PaymentStatus)
		{
			var order = dbContext.OrderHeaders.FirstOrDefault(x => x.Id ==  Id);
			if (order != null)
			{
				order.OrderStatus = OrderStatus;
				order.PaymentDate = DateTime.Now;
				if(PaymentStatus != null)
				order.PaymentStatus = PaymentStatus;
			}
		}
	}
}
