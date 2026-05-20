using Microsoft.AspNetCore.Mvc;
using TripleTBE.Models;
using TripleTBE.Repositories.Interfaces;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartRepository _repository;

        public CartsController(ICartRepository repository)
        {
            _repository = repository;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var carts = await _repository.GetAllAsync();

            return Ok(carts);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cart = await _repository.GetByIdAsync(id);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(Cart cart)
        {
            var createdCart = await _repository.CreateAsync(cart);

            return Ok(createdCart);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Cart cart)
        {
            var updatedCart = await _repository.UpdateAsync(id, cart);

            if (updatedCart == null)
                return NotFound();

            return Ok(updatedCart);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok("Delete success");
        }
    }
}