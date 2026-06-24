using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BadmintonDbContext _context;

        public OrdersController(BadmintonDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // DTOs TRẢ VỀ CHO FRONTEND (Đã chuẩn hóa kiểu dữ liệu)
        // ==========================================
        public class OrderResponseDto
        {
            public int OrderId { get; set; }
            public int? UserId { get; set; }
            public DateTime? OrderDate { get; set; }
            public string? OrderStatus { get; set; }
            public decimal? TotalAmount { get; set; }
            public string PaymentMethod { get; set; } = "COD";
            public List<OrderDetailResponseDto> Items { get; set; } = new();
        }

        public class OrderDetailResponseDto
        {
            public string? ProductName { get; set; }
            public int? Quantity { get; set; }
            public decimal? UnitPrice { get; set; }
            public string? Color { get; set; }
            public string? Size { get; set; }
            public string? Version { get; set; }
            public string? Image { get; set; }
        }
        public class UpdateOrderStatusDto
        {
            public string Status { get; set; } = null!;
        }
        // ==========================================
        // CÁC API ENDPOINTS
        // ==========================================
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            // 1. Tìm đơn hàng cần cập nhật trong database
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound(new { message = $"Không tìm thấy đơn hàng có mã #{id}." });
            }

            // 2. Kiểm tra chuỗi trạng thái gửi lên từ client
            if (string.IsNullOrEmpty(dto.Status))
            {
                return BadRequest(new { message = "Trạng thái đơn hàng không được để trống." });
            }

            // Chuẩn hóa chuỗi (Viết hoa chữ cái đầu hoặc tùy biến khớp với DB của bạn)
            string targetStatus = dto.Status.Trim();

            // 3. Ràng buộc kiểm tra nghiêm ngặt 3 trạng thái hợp lệ
            if (targetStatus != "Pending" && targetStatus != "Completed" && targetStatus != "Declined")
            {
                return BadRequest(new
                {
                    message = "Trạng thái không hợp lệ! Chỉ chấp nhận các giá trị: 'Pending', 'Completed', hoặc 'Declined'."
                });
            }

            // 4. Cập nhật và lưu thay đổi
            order.OrderStatus = targetStatus;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật trạng thái đơn hàng thành công!",
                orderId = order.OrderId,
                newStatus = order.OrderStatus
            });
        }
        // 1. GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Thay thế tên quan hệ 'ProductVariant' nếu trong DB của bạn đặt tên khác (ví dụ: Variant)
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant) // <-- Nếu lỗi, đổi thành od.Variant hoặc od.ProductVariantNavigation
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.Payments)
                .Select(o => new OrderResponseDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.Payments.FirstOrDefault() != null ? o.Payments.FirstOrDefault().PaymentMethod : "COD",
                    Items = o.OrderDetails.Select(od => new OrderDetailResponseDto
                    {
                        ProductName = od.ProductName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        // FIX: Lấy Color, Size, Version từ bảng ProductVariant thay vì OrderDetail
                        Color = od.Variant != null ? od.Variant.Color : "",
                        Size = od.Variant != null ? od.Variant.Size : "",
                        Version = od.Variant != null ? od.Variant.Version : "",
                        Image = od.Variant != null && od.Variant.Product != null
                                ? od.Variant.Product.Thumbnail
                                : "https://via.placeholder.com/150"
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // 2. GET BY USER ID 
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var userOrders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.Payments)
                .Select(o => new OrderResponseDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.Payments.FirstOrDefault() != null ? o.Payments.FirstOrDefault().PaymentMethod : "COD",
                    Items = o.OrderDetails.Select(od => new OrderDetailResponseDto
                    {
                        ProductName = od.ProductName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        // FIX: Lấy dữ liệu thuộc tính từ ProductVariant trung gian
                        Color = od.Variant != null ? od.Variant.Color : "",
                        Size = od.Variant != null ? od.Variant.Size : "",
                        Version = od.Variant != null ? od.Variant.Version : "",
                        Image = od.Variant != null && od.Variant.Product != null
                                ? od.Variant.Product.Thumbnail
                                : "https://via.placeholder.com/150"
                    }).ToList()
                })
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(userOrders);
        }

        // 3. GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.Payments)
                .Select(o => new OrderResponseDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.Payments.FirstOrDefault() != null ? o.Payments.FirstOrDefault().PaymentMethod : "COD",
                    Items = o.OrderDetails.Select(od => new OrderDetailResponseDto
                    {
                        ProductName = od.ProductName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        Color = od.Variant != null ? od.Variant.Color : "",
                        Size = od.Variant != null ? od.Variant.Size : "",
                        Version = od.Variant != null ? od.Variant.Version : "",
                        Image = od.Variant != null && od.Variant.Product != null
                                ? od.Variant.Product.Thumbnail
                                : "https://via.placeholder.com/150"
                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // 4. CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                TotalAmount = dto.TotalAmount,
                OrderStatus = string.IsNullOrEmpty(dto.OrderStatus) ? "Pending" : dto.OrderStatus,
                OrderDate = DateTime.Now // FIX: Gán trực tiếp DateTime.Now không bị lỗi Nullable
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // 5. UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Order updatedOrder)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
                return NotFound();

            if (updatedOrder.UserId > 0) order.UserId = updatedOrder.UserId;
            if (!string.IsNullOrEmpty(updatedOrder.OrderStatus)) order.OrderStatus = updatedOrder.OrderStatus;

            // FIX: Gán giá trị từ Nullable sang Nullable bằng cách kiểm tra HasValue
            if (updatedOrder.TotalAmount.HasValue)
            {
                order.TotalAmount = updatedOrder.TotalAmount.Value;
            }

            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // 6. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Delete order success" });
        }
    }

    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public string? OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
    }
}