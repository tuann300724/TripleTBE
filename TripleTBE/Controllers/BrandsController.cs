using Microsoft.AspNetCore.Mvc;
using TripleTBE.Repositories.Interfaces;
using TripleTBE.Models;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandRepository _repository;

        public BrandsController(IBrandRepository repository)
        {
            _repository = repository;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _repository.GetAllBrands();

            return Ok(brands);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _repository.GetByIdBrand(id);

            if (brand == null)
                return NotFound();

            return Ok(brand);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] Brand brand,
            IFormFile? logo)
        {
            var result = await _repository.CreateBrand(
                brand,
                logo);

            return Ok(result);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] Brand brand,
            IFormFile? logo)
        {
            var result = await _repository.UpdateBrand(
                id,
                brand,
                logo);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

       
    }
}