namespace OrdersManagement.Models.JsonModel
{
    public class JsonOrder
    {
        public int OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreateDate { get; set; }
        public List<JsonOrderLine> OrderLines { get; set; }
    }
}
