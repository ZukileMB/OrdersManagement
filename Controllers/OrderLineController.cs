using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersManagement.Models;
using OrdersManagement.Models.ViewModels;
using OrdersManagement.Repositories;

namespace OrdersManagement.Controllers
{
    public class OrderLineController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderLineController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public IActionResult GetOrderLinePartial(int orderId, int lineNumber = 0)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
            {
                return NotFound();
            }
            var line = order.OrderLines.Where(a => a.LineNumber == lineNumber).FirstOrDefault() ?? new OrderLine();
            
            var prodTypes = new List<SelectListItem>() {
                new SelectListItem {Value="Apparel",Text= "Apparel" },
                new SelectListItem {Value = "Parts", Text = "Parts"},
                new SelectListItem {Value = "Equipment", Text = "Equipment"},
                new SelectListItem {Value = "Motor", Text = "Motor"}
            };
            var model = new OrderLineViewModel
            {
                OrderNumber = order.OrderNumber,
                LineNumber = lineNumber,
                CostPrice = line.CostPrice,
                ProductCode = line.ProductCode,
                ProductType = line.ProductType,
                Quantity = line.Quantity,
                SalesPrice = line.SalesPrice,
            };
            ViewBag.DropdownItems = prodTypes;

            return PartialView("Views/OrderLine/_AddOrderLinePartial.cshtml", model);
        }

        [HttpGet]
        public IActionResult GetOrderLines(int OrderNumber)
        {
            var order = _orderRepository.GetOrder(OrderNumber);
            List<OrderLineViewModel> list = new List<OrderLineViewModel>();
            if (order != null)
            {
                OrderLineViewModel model = new OrderLineViewModel();
                var orderLines = order.OrderLines.Where(ol => ol.OrderNumber == OrderNumber).ToList();
                if (orderLines.Count > 1)
                {
                    foreach (var line in orderLines)
                    {
                        model.OrderNumber = line.OrderNumber;
                        model.LineNumber = line.LineNumber;
                        model.CostPrice = line.CostPrice;
                        model.ProductCode = line.ProductCode;
                        model.ProductType = line.ProductType;
                        model.Quantity = line.Quantity;
                        model.SalesPrice = line.SalesPrice;
                        
                        list.Add(model);
                    }
                }

                return Json(list);
            }
            return Json(BadRequest());
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderLine(OrderLineViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = _orderRepository.GetOrder(model.OrderNumber);
                if (order == null)
                {
                    return NotFound();
                }

                var orderLine = new OrderLine
                {
                    OrderNumber = order.OrderNumber,
                    ProductCode = model.ProductCode,
                    ProductType = model.ProductType,
                    CostPrice = model.CostPrice,
                    SalesPrice = model.SalesPrice,
                    Quantity = model.Quantity
                };

                order.OrderLines.Add(orderLine);
                await _orderRepository.AddOrderLine(orderLine);

                return RedirectToAction("Index", "Order");
            }

            return PartialView("_AddOrderLinePartial", model);
        }

        [HttpGet]
        public IActionResult EditOrderLine(int orderId, int lineId)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            var orderLine = order.OrderLines.FirstOrDefault(ol => ol.LineNumber == lineId);
            if (orderLine == null)
            {
                return Json(new { success = false, message = "Order line not found" });
            }

            var model = new OrderLineViewModel
            {
                OrderNumber = order.OrderNumber,
                LineNumber = orderLine.LineNumber,
                ProductCode = orderLine.ProductCode,
                ProductType = orderLine.ProductType,
                CostPrice = orderLine.CostPrice,
                SalesPrice = orderLine.SalesPrice,
                Quantity = orderLine.Quantity
            };

            return Json(new { success = true, data = model });
        }

        [HttpPost]
        public IActionResult EditOrderLine(OrderLineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            var order = _orderRepository.GetOrder(model.OrderNumber);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            var orderLine = order.OrderLines.FirstOrDefault(ol => ol.LineNumber == model.LineNumber);
            if (orderLine == null)
            {
                return Json(new { success = false, message = "Order line not found" });
            }

            orderLine.ProductCode = model.ProductCode;
            orderLine.ProductType = model.ProductType;
            orderLine.CostPrice = model.CostPrice;
            orderLine.SalesPrice = model.SalesPrice;
            orderLine.Quantity = model.Quantity;

            _orderRepository.UpdateOrderLine(orderLine);

            return Json(new { success = true, message = "Order line updated successfully" });
        }

        [HttpPost]
        public IActionResult DeleteOrderLine(int orderNumber, int lineNumber)
        {
            try
            {
                _orderRepository.DeleteOrderLine(lineNumber, orderNumber);
            }
            catch
            {
                return Json(new { success = false, message = "Order line not found" });
            }
            return RedirectToAction("Index", "Order");
        }
    }
}
