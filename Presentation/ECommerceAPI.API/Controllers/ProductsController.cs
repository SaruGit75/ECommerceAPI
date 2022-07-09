using System.Net;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.RequestParameters;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Application.ViewModels.Products;
using ECommerceAPI.Domain.Entities;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;
        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Pagination pagination)
        {
            #region CommentedRegion
            /*await _productWriteRepository.AddRangeAsync(new()
            {
                new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 25000, CreatedDate = DateTime.Now, Stock = 12 },
                new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 12344, CreatedDate = DateTime.Now, Stock = 123 },
                new() { Id = Guid.NewGuid(), Name = "Product 3", Price = 4324, CreatedDate = DateTime.Now, Stock = 142 },
                new() { Id = Guid.NewGuid(), Name = "Product 4", Price = 2422, CreatedDate = DateTime.Now, Stock = 4 },
            });

            await _productWriteRepository.SaveAsync();*/

            //var p = await _productReadRepository.GetByIdAsync("f2f98e35-e9ad-485f-876a-eab5a3ea1652", false);   //burada tracking mekanizması kapalı oldugundan takip kapalı. Yapilan herhangi bir degisiklik db ye yansimaz.
            //var p = await _productReadRepository.GetByIdAsync("f2f98e35-e9ad-485f-876a-eab5a3ea1652");    //burada ise tam tersi. default deger true oldugundan degisiklikler yansir.
            //p.Name = "Mehmet";
            //await _productWriteRepository.SaveAsync();

            /*await _productWriteRepository.AddAsync(new () { Name = "C product", Price = 1.500F, Stock = 12, CreatedDate = DateTime.Now});
            await _productWriteRepository.SaveAsync();*/
            /*var customerId = Guid.NewGuid();
            await _customerWriteRepository.AddAsync(new() { Id = customerId, Name = "Miuddddddsad" });


            await _orderWriteRepository.AddAsync(new() { Description = "blablalba", Address = "Kocaeli, Yarımca", CustomerId = customerId});
            await _orderWriteRepository.AddAsync(new() { Description = "blablalba2", Address = "Kocaeli, Derince", CustomerId = customerId });
            await _orderWriteRepository.SaveAsync();*/

            //var order = await _orderReadRepository.GetByIdAsync("2adc2ea1-cadc-4c41-86c6-281bf1977d2c");
            //order.Address = "Kocaelii";
            //await _orderWriteRepository.SaveAsync();
            #endregion


            var totalCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false).Skip(pagination.Page * pagination.Size).Take(pagination.Size).Select(t => new
            {
                t.Id,
                t.Name,
                t.Stock,
                t.Price,
                t.CreatedDate,
                t.UpdatedDate
            }).ToList();

            return Ok(new
            {
                totalCount,
                products
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id, false)); //herhangi bir db islemi yok. o yuzden tracking = false
        }


        [HttpPost]
        public async Task<IActionResult> Post(VM_Create_Product product)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            });

            await _productWriteRepository.SaveAsync();

            return StatusCode((int)HttpStatusCode.Created);
        }


        [HttpPut]
        public async Task<IActionResult> Put(VM_Update_Product product)
        {
            var productObject = _productReadRepository.GetByIdAsync(product.Id);
            //alt kısım yemeyebilir, eger yemezse ozellikle Product classını bildir.
            productObject.Result.Stock = product.Stock;
            productObject.Result.Name = product.Name;
            productObject.Result.Price = product.Price;

            _productWriteRepository.SaveAsync();

            return StatusCode((int)HttpStatusCode.NoContent);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();

            return Ok();
        }

        [HttpPost("[action]")]  //zorunlu olarak endpoint'in sonuna action bilgisi girilmesi gerekiyor.
        public async Task<IActionResult> Upload()
        {
            _fileService.UploadAsync("resource/product-images", Request.Form.Files);
            return Ok();
        }
    }
}
