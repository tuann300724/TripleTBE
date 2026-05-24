using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public PaymentsController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _context.Payments
                .Include(x => x.Order)
                .ToListAsync();

            return Ok(payments);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _context.Payments
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.PaymentId == id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // CREATE
        // CREATE PAYMENT
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePaymentDto dto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x =>
                    x.OrderId == dto.OrderId);

            if (order == null)
            {
                return BadRequest(new
                {
                    message = "Order not found"
                });
            }

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentStatus,
                PaymentDate = DateTime.Now
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Create payment success",
                data = payment
            });
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] Payment updatedPayment)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.PaymentId == id);

            if (payment == null)
                return NotFound();

            // update order
            if (updatedPayment.OrderId > 0)
            {
                payment.OrderId = updatedPayment.OrderId;
            }

            // update method
            if (!string.IsNullOrEmpty(updatedPayment.PaymentMethod))
            {
                payment.PaymentMethod =
                    updatedPayment.PaymentMethod;
            }

            // update status
            if (!string.IsNullOrEmpty(updatedPayment.PaymentStatus))
            {
                payment.PaymentStatus =
                    updatedPayment.PaymentStatus;
            }

            // update date
            if (updatedPayment.PaymentDate != null)
            {
                payment.PaymentDate =
                    updatedPayment.PaymentDate;
            }

            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.PaymentId == id);

            if (payment == null)
                return NotFound();

            _context.Payments.Remove(payment);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete payment success"
            });
        }
        // PATCH: api/Payments/ConfirmMomo/5
        [HttpPatch("ConfirmMomo/{orderId}")]
        public async Task<IActionResult> ConfirmMomoPayment(int orderId)
        {
            // 1. Tìm thông tin giao dịch theo OrderId
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (payment == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin thanh toán cho đơn hàng này." });
            }

            // 2. Cập nhật trạng thái thanh toán
            payment.PaymentStatus = "Success";
            payment.PaymentDate = DateTime.Now;

            // 3. Cập nhật đồng bộ trạng thái Đơn hàng (Order) sang "Processing" (Đang xử lý)
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order != null)
            {
                order.OrderStatus = "Processing";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật trạng thái thanh toán MoMo thành công!",
                paymentStatus = payment.PaymentStatus,
                orderStatus = order?.OrderStatus
            });
        }
    }
    public class CreatePaymentDto
    {
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }
    }
}