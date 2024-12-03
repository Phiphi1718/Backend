using Microsoft.EntityFrameworkCore;
using WebCoffee.Data;
using WebCoffee.Models;

namespace WebCoffee.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderVM>> GetAllOrdersAsync();
        Task<Order> AddOrderAsync(OrderMD orderModel, List<OrderDetailsMD> orderDetails);
        Task<Order> UpdateOrderAsync(int id, OrderMD updatedOrder, List<OrderDetailsMD> updatedOrderDetails);
        Task<bool> DeleteOrderAsync(int id);
    }
    public class OrderRepository : IOrderRepository
    {
        private readonly CoffeeHouseDbContext _context;

        public OrderRepository(CoffeeHouseDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả hóa đơn kèm chi tiết
        public async Task<IEnumerable<OrderVM>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .Select(o => new OrderVM
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalItems = o.TotalItems,
                    TotalAmount = o.TotalAmount,
                    PaymentId = o.PaymentId,
                    Payment = o.Payment,
                    User = o.User,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetaildsVM
                    {
                        Id = od.Id,
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        ToppingId = od.ToppingId,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        ToppingPrice = od.ToppingPrice,
                        TotalPrice = od.TotalPrice,
                        Product = od.Product,
                        Topping = od.Topping
                    }).ToList() // Chuyển đổi sang danh sách OrderDetaildsVM
                })
                .ToListAsync();
        }


        // Thêm hóa đơn mới kèm chi tiết
        public async Task<Order> AddOrderAsync(OrderMD orderModel, List<OrderDetailsMD> orderDetails)
        {
            var order = new Order
            {
                UserId = orderModel.UserId,
                OrderDate = orderModel.OrderDate ?? DateTime.Now,
                TotalItems = orderModel.TotalItems,
                TotalAmount = orderModel.TotalAmount,
                PaymentId = orderModel.PaymentId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var detail in orderDetails)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = detail.ProductId,
                    ToppingId = detail.ToppingId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    ToppingPrice = detail.ToppingPrice,
                    TotalPrice = detail.TotalPrice
                };
                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        // Sửa hóa đơn và chi tiết
        public async Task<Order> UpdateOrderAsync(int id, OrderMD updatedOrder, List<OrderDetailsMD> updatedOrderDetails)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return null;
            }

            order.UserId = updatedOrder.UserId;
            order.OrderDate = updatedOrder.OrderDate;
            order.TotalItems = updatedOrder.TotalItems;
            order.TotalAmount = updatedOrder.TotalAmount;
            order.PaymentId = updatedOrder.PaymentId;

            // Xóa chi tiết cũ
            _context.OrderDetails.RemoveRange(order.OrderDetails);

            // Thêm chi tiết mới
            foreach (var detail in updatedOrderDetails)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = id,
                    ProductId = detail.ProductId,
                    ToppingId = detail.ToppingId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    ToppingPrice = detail.ToppingPrice,
                    TotalPrice = detail.TotalPrice
                };
                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        // Xóa hóa đơn và chi tiết
        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return false;
            }

            _context.OrderDetails.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
