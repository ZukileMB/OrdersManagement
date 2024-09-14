using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OrdersManagement.Models.ViewModels
{
    public class OrderLineViewModel
    {
        public int OrderNumber { get; set; }
        public int LineNumber { get; set; }

        [Display(Name = "Product Code")]
        [Required(ErrorMessage = "Product Code is required")]
        [StringLength(50, ErrorMessage = "Product Code cannot exceed 50 characters")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Product Type is required")]
        [Display(Name = "Product Type")]
        public string ProductType { get; set; }

        [Required(ErrorMessage = "Cost Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost Price must be greater than zero")]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Required(ErrorMessage = "Sales Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sales Price must be greater than zero")]
        [Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer")]
        public int Quantity { get; set; }
    }
}