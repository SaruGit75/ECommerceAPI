using ECommerceAPI.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
        }

        [HttpGet]
        public async Task Get()
        {
            await _productWriteRepository.AddRangeAsync(new()
            {
                new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 25000, CreatedDate = DateTime.Now, Stock = 12 },
                new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 12344, CreatedDate = DateTime.Now, Stock = 123 },
                new() { Id = Guid.NewGuid(), Name = "Product 3", Price = 4324, CreatedDate = DateTime.Now, Stock = 142 },
                new() { Id = Guid.NewGuid(), Name = "Product 4", Price = 2422, CreatedDate = DateTime.Now, Stock = 4 },
            });

            await _productWriteRepository.SaveAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var product = await _productReadRepository.GetByIdAsync(id);
            return Ok(product);
        }
    }
}
