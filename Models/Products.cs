using System.ComponentModel.DataAnnotations;

namespace OrdersManagement.Models
{
    public class Products
    {
        [Key]
        public int productCode { get; set; }
        public string productName { get; set; }

        public OrderLine OrderLine { get; set; }
    }
}
