using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrdersManagement.Models;
using OrdersManagement.Models.JsonModel;

namespace OrdersManagement.Repositories
{
    public class JsonOrderRepository : IOrderRepository
    {
        private readonly string filePath;

        public JsonOrderRepository(string filePath)
        {
            this.filePath = filePath;
        }

        private List<Order> LoadOrders()
        {
            if (!File.Exists(filePath))
            {
                return new List<Order>();
            }

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
        }

        /// <summary>
        /// This method is responsible for the save event on the JSON file
        /// it gets list of order then filter the Order lines to prevent nested save
        /// </summary>
        /// <param name="orders"></param>
        private void SaveOrders(List<Order> orders)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            List<JsonOrder> jsonOrders = orders.Select(order => new JsonOrder
            {
                OrderNumber = order.OrderNumber,
                CustomerName = order.CustomerName,
                CreateDate = order.CreateDate,
                OrderStatus= order.OrderStatus,
                OrderType = order.OrderType,

                OrderLines = order.OrderLines.Select(orderLine => new JsonOrderLine
                {
                    LineNumber = orderLine.LineNumber,
                    OrderNumber = orderLine.OrderNumber,
                    ProductCode = orderLine.ProductCode,
                    ProductType = orderLine.ProductType,
                    CostPrice = orderLine.CostPrice,
                    SalesPrice = orderLine.SalesPrice,
                    Quantity = orderLine.Quantity
                }).ToList()

            }).ToList();


            var json = JsonConvert.SerializeObject(jsonOrders, settings);
            File.WriteAllText(filePath, json);
        }

        public IEnumerable<Order> GetOrders()
        {
            return LoadOrders();
        }

        public Order GetOrder(int orderNumber)
        {
            return LoadOrders().FirstOrDefault(order => order.OrderNumber == orderNumber);
        }

        public async Task AddOrderAsync(Order order)
        {
            var existingOrders = LoadOrders();

            if (!existingOrders.Any(x => x.OrderNumber == order.OrderNumber))
            {
                existingOrders.Add(order);
                await Task.Run(() => SaveOrders(existingOrders));
            }
        }

        public void UpdateOrder(Order order)
        {
            var existingOrders = LoadOrders();

            foreach (var item in existingOrders.Where(a => a.OrderNumber == order.OrderNumber))
            {
                item.OrderNumber = order.OrderNumber;
                item.OrderLines.AddRange(order.OrderLines);
                item.CustomerName = order.CustomerName;
                item.OrderStatus = order.OrderStatus;
                item.CreateDate = order.CreateDate;
                item.OrderType = order.OrderType;
            }

            SaveOrders(existingOrders);
        }

        public void DeleteOrder(int orderNumber)
        {
            var orders = LoadOrders();
            var order = orders.Find(o => o.OrderNumber == orderNumber);
            if (order != null)
            {
                order.OrderLines.Clear();

                orders.Remove(order);
                SaveOrders(orders);
            }
        }

        public async Task SaveChangesAsync()
        {
            await Task.CompletedTask;
        }

        public IEnumerable<OrderLine> GetOrderLinesByOrderId(int orderId)
        {
            var order = GetOrder(orderId);
            return order?.OrderLines ?? new List<OrderLine>();
        }

        public async Task AddOrderLine(OrderLine orderLine)
        {
            var orders = LoadOrders();
            if (orders != null)
            {
                foreach (var item in orders.Where(a => a.OrderNumber == orderLine.OrderNumber))
                {
                    item.OrderLines.Add(orderLine);
                }
                SaveOrders(orders);
            }
        }

        public void UpdateOrderLine(OrderLine orderLine)
        {
            var orders = LoadOrders();

            foreach (var order in orders.Where(a => a.OrderNumber == orderLine.OrderNumber))
            {
                foreach (var item in order.OrderLines.Where(a => a.LineNumber == orderLine.LineNumber))
                {
                    item.OrderNumber = orderLine.OrderNumber;
                    item.LineNumber = orderLine.LineNumber;
                    item.ProductCode = orderLine.ProductCode;
                    item.ProductType = orderLine.ProductType;
                    item.CostPrice = orderLine.CostPrice;
                    item.Quantity = orderLine.Quantity;
                }
            }
            SaveOrders(orders);
        }

        public void DeleteOrderLine(int lineNumber, int orderNumber)
        {
            var orders = LoadOrders();
            foreach (var order in orders.Where(a => a.OrderNumber == orderNumber))
            {
                var orderLine = order.OrderLines.Find(line => line.LineNumber == lineNumber);
                if (orderLine != null)
                {
                    order.OrderLines.Remove(orderLine);
                  
                }
            }
            SaveOrders(orders);
        }

        public bool OrderLineExists(int orderId, string product)
        {
            var order = GetOrder(orderId);
            return order?.OrderLines.Any(line => line.ProductCode == product) ?? false;
        }
    }
}
