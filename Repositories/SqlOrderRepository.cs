using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrdersManagement.Data.OrderContext;
using OrdersManagement.Models;

namespace OrdersManagement.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly OrderManagementContext _context;

        public SqlOrderRepository(OrderManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetOrders()
        {
            return _context.Orders.Include(o => o.OrderLines).ToList();
        }

        public Order GetOrder(int orderNumber)
        {
            return _context.Orders.Include(o => o.OrderLines).FirstOrDefault(o => o.OrderNumber == orderNumber);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await SaveChangesAsync();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void DeleteOrder(int orderNumber)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
            if (order != null)
            {
                var orderLine = _context.OrderLines.Where(a=>a.OrderNumber == orderNumber).ToList();
                foreach (var line in orderLine)
                {
                    _context.Remove(line);
                }

                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IEnumerable<OrderLine> GetOrderLinesByOrderId(int orderId)
        {
            return _context.OrderLines.Where(ol => ol.OrderNumber == orderId).ToList();
        }

        public async Task AddOrderLine(OrderLine orderLine)
        {
            await _context.OrderLines.AddAsync(orderLine);
            await _context.SaveChangesAsync();
        }

        public void UpdateOrderLine(OrderLine orderLine)
        {
            _context.OrderLines.Update(orderLine);
            _context.SaveChanges();
        }

        public void DeleteOrderLine(int lineNumber, int orderNumber)
        {
            Order? Orders = _context.Orders.Find(orderNumber);
            if (Orders != null)
            {
                OrderLine? orderLine = _context.OrderLines.Where(a => a.LineNumber == lineNumber && a.OrderNumber == orderNumber).FirstOrDefault();
                if (orderLine != null)
                {
                    _context.OrderLines.Remove(orderLine);
                    _context.SaveChanges();
                }
            }
        }

        public bool OrderLineExists(int orderId, string product)
        {
            return _context.OrderLines.Any(ol => ol.OrderNumber == orderId && ol.ProductCode == product);
        }
    }
}
