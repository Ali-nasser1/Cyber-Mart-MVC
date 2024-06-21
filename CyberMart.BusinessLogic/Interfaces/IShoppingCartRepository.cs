using CyberMart.DataAccess.Models;
using Microsoft.Identity.Client;

namespace CyberMart.BusinessLogic.Interfaces
{
	public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
	{
		public int IncreaseCount(ShoppingCart shoppingCart, int count);
		public int DecreaseCount(ShoppingCart shoppingCart, int count);
	}
}
