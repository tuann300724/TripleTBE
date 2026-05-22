using Microsoft.AspNetCore.Mvc;
using TripleTBE.DTOs;
using TripleTBE.Models;
using TripleTBE.Repositories.Interfaces;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(
            IProductRepository repository)
        {
            _repository = repository;
        }

        /* =========================================================
           GET ALL
        ========================================================= */

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync();

            return Ok(products);
        }

        /* =========================================================
           GET BY ID
        ========================================================= */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(product);
        }

        /* =========================================================
           CREATE PRODUCT
        ========================================================= */

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromForm] ProductCreateDTO dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,

                Description = dto.Description,

                BrandId = dto.BrandId,

                CategoryId = dto.CategoryId,

                Status = dto.Status ?? 1,

                CreatedAt = DateTime.Now,

                UpdatedAt = DateTime.Now
            };

            var createdProduct = await _repository.CreateAsync(
                product,
                dto.Thumbnail,
                dto.Images);

            return Ok(createdProduct);
        }

        /* =========================================================
           UPDATE PRODUCT
        ========================================================= */

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromForm] ProductCreateDTO dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,

                Description = dto.Description,

                BrandId = dto.BrandId,

                CategoryId = dto.CategoryId,

                Status = dto.Status
            };

            var updatedProduct = await _repository.UpdateAsync(
                id,
                product,
                dto.Thumbnail,
                dto.Images);

            if (updatedProduct == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(updatedProduct);
        }

        /* =========================================================
           CHANGE STATUS
        ========================================================= */

        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> ChangeStatus(
            int id,
            [FromQuery] int status)
        {
            var result = await _repository
                .ChangeStatusAsync(id, status);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(new
            {
                message = "Updated status successfully"
            });
        }
    }
}