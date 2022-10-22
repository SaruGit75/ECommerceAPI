using MediatR;

namespace ECommerceAPI.Application.Features.Queries.Product.GetByIdProduct;

public class GetByIdProductRequest : IRequest<GetByIdProductResponse>
{
    public string Id { get; set; }
}