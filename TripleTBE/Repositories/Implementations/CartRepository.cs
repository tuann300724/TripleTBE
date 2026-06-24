using Microsoft.EntityFrameworkCore;

using TripleTBE.Models;
using TripleTBE.Repositories.Interfaces;

namespace TripleTBE.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly BadmintonDbContext _context;

        public CartRepository(BadmintonDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems)
                .ToListAsync();
        }

        // GET BY ID
        public async Task<Cart?> GetByIdAsync(int id)
        {
            return await _context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == id);
        }

        // CREATE
        public async Task<Cart> CreateAsync(Cart cart)
        {
            cart.CreatedAt = DateTime.Now;

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        // UPDATE
        public async Task<Cart?> UpdateAsync(int id, Cart updatedCart)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null)
                return null;

            if (updatedCart.UserId > 0)
            {
                cart.UserId = updatedCart.UserId;
            }

            await _context.SaveChangesAsync();

            return cart;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null)
                return false;

            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task AddToCartAsync(
    int userId,
    int variantId,
    int quantity)
        {
            // tìm cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == userId);

            // chưa có cart
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };

                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();
            }

            // tìm cart item
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.CartId == cart.CartId &&
                    x.VariantId == variantId);

            // có rồi -> cộng số lượng
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                // chưa có -> tạo mới
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    VariantId = variantId,
                    Quantity = quantity
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }
    }
}