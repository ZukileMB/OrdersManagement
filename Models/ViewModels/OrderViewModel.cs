using System.ComponentModel.DataAnnotations;

namespace OrdersManagement.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }

        [Required(ErrorMessage = "Order Type is required")]
        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Required(ErrorMessage = "Order Status is required")]
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [StringLength(100, ErrorMessage = "Customer Name cannot exceed 100 characters")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Create Date is required")]
        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }

        public List<OrderLineViewModel> OrderLines { get; set; } = new List<OrderLineViewModel>();
    }
}
