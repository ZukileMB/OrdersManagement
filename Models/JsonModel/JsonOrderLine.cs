namespace OrdersManagement.Models.JsonModel
{
    public class JsonOrderLine
    {
        public int LineNumber { get; set; }
        public int OrderNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalesPrice { get; set; }
        public int Quantity { get; set; }
    }
}
