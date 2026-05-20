using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public OrderDetailsController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orderDetails = await _context.OrderDetails
                .Include(x => x.Order)
                .Include(x => x.Product)
                .Include(x => x.Variant)
                .ToListAsync();

            return Ok(orderDetails);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(x => x.Order)
                .Include(x => x.Product)
                .Include(x => x.Variant)
                .FirstOrDefaultAsync(x => x.OrderDetailId == id);

            if (orderDetail == null)
                return NotFound();

            return Ok(orderDetail);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] OrderDetail orderDetail)
        {
            orderDetail.TotalPrice =
                orderDetail.Quantity * orderDetail.UnitPrice;

            _context.OrderDetails.Add(orderDetail);

            await _context.SaveChangesAsync();

            return Ok(orderDetail);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] OrderDetail updatedOrderDetail)
        {
            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(x => x.OrderDetailId == id);

            if (orderDetail == null)
                return NotFound();

            // update order
            if (updatedOrderDetail.OrderId > 0)
            {
                orderDetail.OrderId = updatedOrderDetail.OrderId;
            }

            // update product
            if (updatedOrderDetail.ProductId > 0)
            {
                orderDetail.ProductId = updatedOrderDetail.ProductId;
            }

            // update variant
            if (updatedOrderDetail.VariantId != null)
            {
                orderDetail.VariantId =
                    updatedOrderDetail.VariantId;
            }

            // update quantity
            if (updatedOrderDetail.Quantity > 0)
            {
                orderDetail.Quantity =
                    updatedOrderDetail.Quantity;
            }

            // update price
            if (updatedOrderDetail.UnitPrice > 0)
            {
                orderDetail.UnitPrice =
                    updatedOrderDetail.UnitPrice;
            }

            // auto total
            orderDetail.TotalPrice =
                orderDetail.Quantity * orderDetail.UnitPrice;

            await _context.SaveChangesAsync();

            return Ok(orderDetail);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(x => x.OrderDetailId == id);

            if (orderDetail == null)
                return NotFound();

            _context.OrderDetails.Remove(orderDetail);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete order detail success"
            });
        }
    }
}