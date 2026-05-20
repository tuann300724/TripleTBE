using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public OrdersController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderDetails)
                .Include(x => x.Payments)
                .ToListAsync();

            return Ok(orders);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderDetails)
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] Order order)
        {
            order.OrderDate = DateTime.Now;

            if (string.IsNullOrEmpty(order.OrderStatus))
            {
                order.OrderStatus = "Pending";
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] Order updatedOrder)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
                return NotFound();

            // update user
            if (updatedOrder.UserId > 0)
            {
                order.UserId = updatedOrder.UserId;
            }

            // update status
            if (!string.IsNullOrEmpty(updatedOrder.OrderStatus))
            {
                order.OrderStatus = updatedOrder.OrderStatus;
            }

            // update total
            if (updatedOrder.TotalAmount != null)
            {
                order.TotalAmount = updatedOrder.TotalAmount;
            }

            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete order success"
            });
        }
    }
}