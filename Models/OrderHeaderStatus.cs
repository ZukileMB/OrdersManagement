using System.ComponentModel.DataAnnotations;

namespace OrdersManagement.Models
{
    public class OrderHeaderStatus
    {
        [Key]
        public int OrderStatusCode { get; set; }
        public string OrderStatus { get; set; }

        public Order order { get; set; }
    }
}
