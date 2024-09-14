using OrdersManagement.Models;
using System.Collections.Generic;

namespace OrdersManagement.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();
        Order GetOrder(int orderNumber);
      
        void UpdateOrder(Order order);
        void DeleteOrder(int orderNumber);
        Task AddOrderAsync(Order order);
        Task SaveChangesAsync();

        IEnumerable<OrderLine> GetOrderLinesByOrderId(int orderId);
        Task AddOrderLine(OrderLine orderLine);
        void UpdateOrderLine(OrderLine orderLine);
        void DeleteOrderLine(int lineNumber,int orderNumber);
        bool OrderLineExists(int orderId, string product);
    }
}
