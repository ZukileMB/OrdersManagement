using System.Collections.Generic;
using System.Threading.Tasks;
using OrdersManagement.Models;

namespace OrdersManagement.Repositories
{
    public class CombinedOrderRepository : IOrderRepository
    {
        private readonly SqlOrderRepository _sqlOrderRepository;
        private readonly JsonOrderRepository _jsonOrderRepository;

        public CombinedOrderRepository(SqlOrderRepository sqlOrderRepository, JsonOrderRepository jsonOrderRepository)
        {
            _sqlOrderRepository = sqlOrderRepository;
            _jsonOrderRepository = jsonOrderRepository;
        }

        public IEnumerable<Order> GetOrders()
        {
            // For simplicity, we are only getting orders from SQL repository
            // You can combine results from both repositories if needed
            return _sqlOrderRepository.GetOrders();
        }

        public Order GetOrder(int orderNumber)
        {
            return _sqlOrderRepository.GetOrder(orderNumber);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _sqlOrderRepository.AddOrderAsync(order);
            await _jsonOrderRepository.AddOrderAsync(order);
        }

        public void UpdateOrder(Order order)
        {
            _sqlOrderRepository.UpdateOrder(order);
            _jsonOrderRepository.UpdateOrder(order);
        }

        public void DeleteOrder(int orderNumber)
        {
            _sqlOrderRepository.DeleteOrder(orderNumber);
            _jsonOrderRepository.DeleteOrder(orderNumber);
        }

        public async Task SaveChangesAsync()
        {
            await _sqlOrderRepository.SaveChangesAsync();
            await _jsonOrderRepository.SaveChangesAsync();
        }

        public IEnumerable<OrderLine> GetOrderLinesByOrderId(int orderId)
        {
            // For simplicity, we are only getting order lines from SQL repository
            // You can combine results from both repositories if needed
            return _sqlOrderRepository.GetOrderLinesByOrderId(orderId);
        }

        public async Task AddOrderLine(OrderLine orderLine)
        {
            await _sqlOrderRepository.AddOrderLine(orderLine);
            await _jsonOrderRepository.AddOrderLine(orderLine);
        }

        public void UpdateOrderLine(OrderLine orderLine)
        {
            _sqlOrderRepository.UpdateOrderLine(orderLine);
            _jsonOrderRepository.UpdateOrderLine(orderLine);
        }

        public void DeleteOrderLine(int lineNumber, int orderNumber)
        {
            _sqlOrderRepository.DeleteOrderLine(lineNumber, orderNumber);
            _jsonOrderRepository.DeleteOrderLine(lineNumber, orderNumber);
        }

        public bool OrderLineExists(int orderId, string product)
        {
            return _sqlOrderRepository.OrderLineExists(orderId, product);
        }
    }
}
