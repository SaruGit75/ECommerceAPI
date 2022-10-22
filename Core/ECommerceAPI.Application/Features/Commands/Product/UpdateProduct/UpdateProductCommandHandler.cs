using ECommerceAPI.Application.Repositories;
using MediatR;
using P = ECommerceAPI.Domain.Entities.Product;

namespace ECommerceAPI.Application.Features.Commands.Product.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly IProductWriteRepository _productWriteRepository;

    public UpdateProductCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
    {
        _productReadRepository = productReadRepository;
        _productWriteRepository = productWriteRepository;
    }

    public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
    {
        P product = await _productReadRepository.GetByIdAsync(request.Id);
        
        product.Stock = request.Stock;
        product.Name = request.Name;
        product.Price = request.Price;

        await _productWriteRepository.SaveAsync();
        return new ();
    }
}