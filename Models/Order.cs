namespace OrdersManagement.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreateDate { get; set; }
        public List<OrderLine> OrderLines { get; set; }
    }
}
