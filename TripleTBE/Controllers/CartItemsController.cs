using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly BadmintonStoreDbContext _context;

        public CartItemsController(BadmintonStoreDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cartItems = await _context.CartItems
                .Include(x => x.Variant)
                .Include(x => x.Cart)
                .ToListAsync();

            return Ok(cartItems);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cartItem = await _context.CartItems
                .Include(x => x.Variant)
                .Include(x => x.Cart)
                .FirstOrDefaultAsync(x => x.CartItemId == id);

            if (cartItem == null)
                return NotFound();

            return Ok(cartItem);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);

            await _context.SaveChangesAsync();

            return Ok(cartItem);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] CartItem updatedCartItem)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.CartItemId == id);

            if (cartItem == null)
                return NotFound();

            // update
            if (updatedCartItem.CartId > 0)
            {
                cartItem.CartId = updatedCartItem.CartId;
            }

            if (updatedCartItem.VariantId != null)
            {
                cartItem.VariantId = updatedCartItem.VariantId;
            }

            if (updatedCartItem.Quantity != null)
            {
                cartItem.Quantity = updatedCartItem.Quantity;
            }

            await _context.SaveChangesAsync();

            return Ok(cartItem);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.CartItemId == id);

            if (cartItem == null)
                return NotFound();

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delete cart item success"
            });
        }
    }
}