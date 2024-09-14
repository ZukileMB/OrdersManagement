using OrdersManagement.Models;
using OrdersManagement.Models.ViewModels;
using OrdersManagement.Repositories;

namespace OrdersManagement.Controllers.ControllerHelper
{
    public class OrdersHelper
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersHelper(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task CreateOrder(OrderViewModel model)
        {
            Order order = new Order
            {
                OrderNumber = model.OrderNumber,
                OrderType = model.OrderType,
                OrderStatus = model.OrderStatus,
                CustomerName = model.CustomerName,
                CreateDate = model.CreateDate,
                OrderLines = new List<OrderLine>()
            };

            foreach (var line in model.OrderLines)
            {
                order.OrderLines.Add(new OrderLine
                {
                    ProductCode = line.ProductCode,
                    ProductType = line.ProductType,
                    CostPrice = line.CostPrice,
                    SalesPrice = line.SalesPrice,
                    Quantity = line.Quantity
                });
            }

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

        public OrderViewModel GetOrderDetails(int orderNumber)
        {
            Order order = _orderRepository.GetOrder(orderNumber);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            OrderViewModel model = new OrderViewModel
            {
                OrderNumber = order.OrderNumber,
                OrderType = order.OrderType,
                OrderStatus = order.OrderStatus,
                CustomerName = order.CustomerName,
                CreateDate = order.CreateDate,
                OrderLines = order.OrderLines.Select(line => new OrderLineViewModel
                {
                    LineNumber = line.LineNumber,
                    ProductCode = line.ProductCode,
                    ProductType = line.ProductType,
                    CostPrice = line.CostPrice,
                    SalesPrice = line.SalesPrice,
                    Quantity = line.Quantity
                }).ToList()
            };

            return model;
        }
    }
}
