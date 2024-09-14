namespace OrdersManagement.Models
{
    public class ProductType
    {

        public int Id { get; set; }
        public string description { get; set; }

        public Products Product { get; set; }
    }
}
