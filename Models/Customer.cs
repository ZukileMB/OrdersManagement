using System.ComponentModel.DataAnnotations;

namespace OrdersManagement.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public int CustomerName { get; set; }

        public Order Order { get; set; }
    }
}
