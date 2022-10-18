using System.Net;
using System.Runtime.CompilerServices;
using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Features.Commands.CreateProduct;
using ECommerceAPI.Application.Features.Queries.GetAllProduct;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.RequestParameters;
using ECommerceAPI.Application.ViewModels.Products;
using ECommerceAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileWriteRepository _fileWriteRepository;
        private readonly IFileReadRepository _fileReadRepository;
        private readonly IProductImageFileReadRepository _productImageFileReadRepository;
        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        private readonly IInvoiceFileReadRepository _invoiceFileReadRepository;
        private readonly IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        private readonly IStorageService _storageService;

        private readonly IMediator _mediator;


        public ProductsController(IProductWriteRepository productWriteRepository,
            IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment,
            IFileWriteRepository fileWriteRepository, IFileReadRepository fileReadRepository,
            IProductImageFileReadRepository productImageFileReadRepository,
            IProductImageFileWriteRepository productImageFileWriteRepository,
            IInvoiceFileReadRepository invoiceFileReadRepository,
            IInvoiceFileWriteRepository invoiceFileWriteRepository, IStorageService storageService, IMediator mediator)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _webHostEnvironment = webHostEnvironment;
            _fileWriteRepository = fileWriteRepository;
            _fileReadRepository = fileReadRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _storageService = storageService;
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return
                Ok(await _productReadRepository.GetByIdAsync(id,
                    false)); //herhangi bir db islemi yok. o yuzden tracking = false
        }


        [HttpPost]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
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

        [HttpPost("[action]")] //zorunlu olarak endpoint'in sonuna action bilgisi girilmesi gerekiyor.
        public async Task<IActionResult> Upload(string id)
        {
            //var datas = await _fileService.UploadAsync("resource/files", Request.Form.Files);

            #region CommentedAreas

            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(t => new ProductImageFile
            //{
            //    FileName = t.fileName,
            //    Path = t.path
            //}).ToList());

            //await _productImageFileWriteRepository.SaveAsync();

            //await _invoiceFileWriteRepository.AddRangeAsync(datas.Select(t => new InvoiceFile()
            //{
            //    FileName = t.fileName,
            //    Path = t.path,
            //    Price = new Random().Next()
            //}).ToList());

            //await _invoiceFileWriteRepository.SaveAsync();


            //await _fileWriteRepository.AddRangeAsync(datas.Select(t => new ECommerceAPI.Domain.Entities.File()
            //{
            //    FileName = t.fileName,
            //    Path = t.path
            //}).ToList());

            //await _fileWriteRepository.SaveAsync();

            //var data1 = _fileReadRepository.GetAll(false);
            //var data2 = _invoiceFileReadRepository.GetAll(false);
            //var data3 = _productImageFileReadRepository.GetAll(false);
            //var getFileDatas = Request.Form.Files.ToList();


            //var datas = await _storageService.UploadAsync("resources/files", Request.Form.Files);

            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(t => new ECommerceAPI.Domain.Entities.ProductImageFile()
            //{
            //    FileName = t.fileName,
            //    Path = t.pathOrContainerName,
            //    Storage = "Local"
            //}).ToList());

            //await _productImageFileWriteRepository.SaveAsync();

            #endregion

            List<(string fileName, string pathOrContainerName)> result =
                await _storageService.UploadAsync("photo-images", Request.Form.Files);


            var product = await _productReadRepository.GetByIdAsync(id);

            await _productImageFileWriteRepository.AddRangeAsync(result.Select(t => new ProductImageFile
            {
                FileName = t.fileName,
                Path = t.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<Product>
                {
                    product
                }
            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();

            return Ok();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProductImages(string id)
        {
            var product = await _productReadRepository.Table
                .Include(t => t.ProductImageFiles)
                .FirstOrDefaultAsync(t => t.Id == Guid.Parse(id));

            return Ok(product?.ProductImageFiles.Select(t => new
            {
                Path = $"D:\\source\\ECommerceAPI\\Presentation\\ECommerceAPI.API\\wwwroot\\{t.Path}",
                t.FileName,
                t.Id
            }));
        }


        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage(string id, string imageId)
        {
            Product? product = await _productReadRepository.Table
                .Include(t => t.ProductImageFiles)
                .FirstOrDefaultAsync(t => t.Id == Guid.Parse(id));

            var productImageFile = product.ProductImageFiles.FirstOrDefault(t => t.Id == Guid.Parse(imageId));

            product.ProductImageFiles.Remove(productImageFile);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }
    }
}