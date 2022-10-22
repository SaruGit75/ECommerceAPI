using ECommerceAPI.Application.Repositories;
using MediatR;

namespace ECommerceAPI.Application.Features.Queries.Product.GetAllProduct;

public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
{
    private readonly IProductReadRepository _productReadRepository;
    public GetAllProductQueryHandler(IProductReadRepository productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }
    public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
    {
        var totalCount = _productReadRepository.GetAll(false).Count();
        var products = _productReadRepository.GetAll(false).Skip(request.Page * request.Size)
            .Take(request.Size).Select(t => new
            {
                t.Id,
                t.Name,
                t.Stock,
                t.Price,
                t.CreatedDate,
                t.UpdatedDate
            }).ToList();

        return new()
        {
            Products = products,
            TotalCount = totalCount
        };
    }
}