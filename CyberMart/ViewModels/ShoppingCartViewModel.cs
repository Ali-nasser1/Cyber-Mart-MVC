using CyberMart.DataAccess.Models;

namespace CyberMart.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> CartsList { get; set; }
        public decimal TotalPrices { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
