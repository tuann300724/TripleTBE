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

        public OrderDetailsController(
            BadmintonStoreDbContext context)
        {
            _context = context;
        }

        /* =========================================================
           GET ALL
        ========================================================= */

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orderDetails = await _context.OrderDetails
                .Include(x => x.Order)
                .Include(x => x.Variant)
                .ThenInclude(v => v.Product)
                .Select(x => new
                {
                    x.OrderDetailId,

                    x.OrderId,

                    x.VariantId,

                    ProductName = x.Variant.Product.ProductName,

                    x.Variant.Color,

                    x.Variant.Size,

                    x.Variant.Version,

                    x.Quantity,

                    x.UnitPrice,

                    TotalPrice = x.Quantity * x.UnitPrice
                })
                .ToListAsync();

            return Ok(orderDetails);
        }

        /* =========================================================
           GET BY ID
        ========================================================= */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(x => x.Order)
                .Include(x => x.Variant)
                .ThenInclude(v => v.Product)
                .Where(x => x.OrderDetailId == id)
                .Select(x => new
                {
                    x.OrderDetailId,

                    x.OrderId,

                    x.VariantId,

                    ProductName = x.Variant.Product.ProductName,

                    x.Variant.Color,

                    x.Variant.Size,

                    x.Variant.Version,

                    x.Quantity,

                    x.UnitPrice,

                    TotalPrice = x.Quantity * x.UnitPrice
                })
                .FirstOrDefaultAsync();

            if (orderDetail == null)
            {
                return NotFound(new
                {
                    message = "Order detail not found"
                });
            }

            return Ok(orderDetail);
        }

        /* =========================================================
           CREATE
        ========================================================= */

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] OrderDetail orderDetail)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v =>
                    v.VariantId == orderDetail.VariantId);

            if (variant == null)
            {
                return BadRequest(new
                {
                    message = "Variant not found"
                });
            }

            if (orderDetail.Quantity <= 0)
            {
                return BadRequest(new
                {
                    message = "Quantity must be greater than 0"
                });
            }

            if (variant.Stock < orderDetail.Quantity)
            {
                return BadRequest(new
                {
                    message = "Not enough stock"
                });
            }

            // snapshot price
            orderDetail.UnitPrice = variant.Price;

            // reduce stock
            variant.Stock -= orderDetail.Quantity;

            _context.OrderDetails.Add(orderDetail);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Create order detail success",
                data = orderDetail
            });
        }

        /* =========================================================
           UPDATE
        ========================================================= */

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] OrderDetail updatedOrderDetail)
        {
            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(x =>
                    x.OrderDetailId == id);

            if (orderDetail == null)
            {
                return NotFound(new
                {
                    message = "Order detail not found"
                });
            }

            if (updatedOrderDetail.Quantity > 0)
            {
                orderDetail.Quantity =
                    updatedOrderDetail.Quantity;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Update order detail success",
                data = orderDetail
            });
        }

        /* =========================================================
           DELETE
        ========================================================= */

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(x =>
                    x.OrderDetailId == id);

            if (orderDetail == null)
            {
                return NotFound(new
                {
                    message = "Order detail not found"
                });
            }

            _context.OrderDetails.Remove(orderDetail);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete order detail success"
            });
        }
    }
}