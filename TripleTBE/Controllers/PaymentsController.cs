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
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] Payment payment)
        {
            payment.PaymentDate = DateTime.Now;

            if (string.IsNullOrEmpty(payment.PaymentStatus))
            {
                payment.PaymentStatus = "Pending";
            }

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return Ok(payment);
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
    }
}