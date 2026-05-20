using Microsoft.EntityFrameworkCore;

using TripleTBE.Models;
using TripleTBE.Repositories.Interfaces;

namespace TripleTBE.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly BadmintonStoreDbContext _context;

        public CartRepository(BadmintonStoreDbContext context)
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
    }
}