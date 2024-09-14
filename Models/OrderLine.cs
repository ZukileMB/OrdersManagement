namespace OrdersManagement.Models
{
    public class OrderLine
    {
        public int LineNumber { get; set; }
        public int OrderNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalesPrice { get; set; }
        public int Quantity { get; set; }

        public Order Order { get; set; }
    }
}
