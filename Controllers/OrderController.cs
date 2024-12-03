using Microsoft.AspNetCore.Mvc;
using WebCoffee.Data;
using WebCoffee.Models;
using WebCoffee.Repository;

namespace WebCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Lấy tất cả hóa đơn kèm chi tiết
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderVM>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi khi lấy danh sách hóa đơn", error = ex.Message });
            }
        }

        // Thêm hóa đơn mới kèm chi tiết
        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder([FromBody] OrderRequest orderRequest)
        {
            try
            {
                if (orderRequest == null || orderRequest.OrderDetails == null || !orderRequest.OrderDetails.Any())
                {
                    return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ" });
                }

                var orderModel = new OrderMD
                {
                    UserId = orderRequest.UserId,
                    OrderDate = orderRequest.OrderDate,
                    TotalItems = orderRequest.TotalItems,
                    TotalAmount = orderRequest.TotalAmount,
                    PaymentId = orderRequest.PaymentId
                };

                var orderDetails = orderRequest.OrderDetails.Select(od => new OrderDetailsMD
                {
                    ProductId = od.ProductId,
                    ToppingId = od.ToppingId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    ToppingPrice = od.ToppingPrice,
                    TotalPrice = od.TotalPrice
                }).ToList();

                var createdOrder = await _orderRepository.AddOrderAsync(orderModel, orderDetails);
                return CreatedAtAction(nameof(GetAllOrders), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi khi thêm hóa đơn", error = ex.Message });
            }
        }

        // Sửa hóa đơn và chi tiết
        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> UpdateOrder(int id, [FromBody] OrderRequest orderRequest)
        {
            try
            {
                if (orderRequest == null || orderRequest.OrderDetails == null || !orderRequest.OrderDetails.Any())
                {
                    return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ" });
                }

                var updatedOrder = new OrderMD
                {
                    UserId = orderRequest.UserId,
                    OrderDate = orderRequest.OrderDate,
                    TotalItems = orderRequest.TotalItems,
                    TotalAmount = orderRequest.TotalAmount,
                    PaymentId = orderRequest.PaymentId
                };

                var updatedOrderDetails = orderRequest.OrderDetails.Select(od => new OrderDetailsMD
                {
                    ProductId = od.ProductId,
                    ToppingId = od.ToppingId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    ToppingPrice = od.ToppingPrice,
                    TotalPrice = od.TotalPrice
                }).ToList();

                var order = await _orderRepository.UpdateOrderAsync(id, updatedOrder, updatedOrderDetails);
                if (order == null)
                {
                    return NotFound(new { message = "Hóa đơn không tồn tại" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi khi cập nhật hóa đơn", error = ex.Message });
            }
        }

        // Xóa hóa đơn
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderRepository.DeleteOrderAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Hóa đơn không tồn tại" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi khi xóa hóa đơn", error = ex.Message });
            }
        }
    }

    // Model để nhận dữ liệu yêu cầu từ client
    public class OrderRequest
    {
        public int UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentId { get; set; }
        public List<OrderDetailRequest> OrderDetails { get; set; }
    }

    // Model cho chi tiết đơn hàng
    public class OrderDetailRequest
    {
        public int ProductId { get; set; }
        public int ToppingId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ToppingPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
