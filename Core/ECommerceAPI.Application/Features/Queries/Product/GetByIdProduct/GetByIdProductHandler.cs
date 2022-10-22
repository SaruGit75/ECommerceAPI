using ECommerceAPI.Application.Repositories;
using MediatR;
using P = ECommerceAPI.Domain.Entities;
namespace ECommerceAPI.Application.Features.Queries.Product.GetByIdProduct;

public class GetByIdProductHandler : IRequestHandler<GetByIdProductRequest, GetByIdProductResponse>
{
    private readonly IProductReadRepository _productReadRepository;

    public GetByIdProductHandler(IProductReadRepository productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }

    public async Task<GetByIdProductResponse> Handle(GetByIdProductRequest request, CancellationToken cancellationToken)
    {
        P.Product? product = await _productReadRepository.GetByIdAsync(request.Id, false);
        
        
        return new()
        {
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}