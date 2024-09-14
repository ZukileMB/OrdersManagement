using Microsoft.AspNetCore.Mvc;
using OrdersManagement.Controllers.ControllerHelper;
using OrdersManagement.Models;
using OrdersManagement.Models.ViewModels;
using OrdersManagement.Repositories;

namespace OrdersManagement.Controllers
{
    //[ApiController]
    // [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private  OrdersHelper _ordersHelper;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _ordersHelper = new OrdersHelper(orderRepository);
        }

        // GET: api/order/{orderNumber}
        [HttpGet("{orderNumber}")]
        public IActionResult GetOrderDetails(int orderNumber)
        {
            try
            {
                OrderViewModel data = _ordersHelper.GetOrderDetails(orderNumber);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Index()
        {
            var orders = _orderRepository.GetOrders();

            List<OrderViewModel> viewModelList = new List<OrderViewModel>();

            foreach (Order order in orders)
            {
                OrderViewModel orderViewModel = new OrderViewModel
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

                viewModelList.Add(orderViewModel);
            }

            return View(viewModelList);
        }

        public IActionResult Create()
        {
            var model = new OrderViewModel
            {
                CreateDate = DateTime.Now,
                OrderLines = new List<OrderLineViewModel>()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    await _ordersHelper.CreateOrder(model);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, errors });
                }
            }
            catch (Exception ex)
            {
                        
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AddOrderLine(int orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderLineViewModel
            {
                OrderNumber = order.OrderNumber
            };

            return PartialView("~/Views/OrderLine/_AddOrderLinePartial",model);
        }

        public IActionResult Edit(int id)
        {
            var order = _orderRepository.GetOrder(id);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderViewModel
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

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = _orderRepository.GetOrder(model.OrderNumber);
                if (order == null)
                {
                    return NotFound();
                }

                order.OrderType = model.OrderType;
                order.OrderStatus = model.OrderStatus;
                order.CustomerName = model.CustomerName;
                order.CreateDate = model.CreateDate;

                order.OrderLines = model.OrderLines.Select(line => new OrderLine
                {
                    LineNumber = line.LineNumber,
                    ProductCode = line.ProductCode,
                    ProductType = line.ProductType,
                    CostPrice = line.CostPrice,
                    SalesPrice = line.SalesPrice,
                    Quantity = line.Quantity
                }).ToList();

                _orderRepository.UpdateOrder(order);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderNumber)
        {
            var order = _orderRepository.GetOrder(orderNumber);
            if (order == null)
            {
                return NotFound();
            }

            _orderRepository.DeleteOrder(orderNumber);

            return RedirectToAction(nameof(Index));
        }
    }
}
