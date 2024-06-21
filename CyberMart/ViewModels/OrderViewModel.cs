using CyberMart.DataAccess.Models;

namespace CyberMart.ViewModels
{
    public class OrderViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrdersDetails { get; set; }

    }
}
