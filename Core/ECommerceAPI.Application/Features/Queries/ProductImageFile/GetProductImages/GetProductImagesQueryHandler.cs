using ECommerceAPI.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using P = ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Features.Queries.ProductImageFile.GetProductImages;

public class GetProductImagesQueryHandler : IRequestHandler<GetProductImagesQueryRequest, List<GetProductImagesQueryResponse>>
{
    private readonly IProductReadRepository _productReadRepository;

    public GetProductImagesQueryHandler(IProductReadRepository productReadRepository)
    {
        _productReadRepository = productReadRepository;
    }

    public async Task<List<GetProductImagesQueryResponse>> Handle(GetProductImagesQueryRequest request,
        CancellationToken cancellationToken)
    {
        P.Product? product = await _productReadRepository.Table
            .Include(t => t.ProductImageFiles)
            .FirstOrDefaultAsync(t => t.Id == Guid.Parse(request.Id));

        return product?.ProductImageFiles.Select(t => new GetProductImagesQueryResponse
        {
            Path = $"D:\\source\\ECommerceAPI\\Presentation\\ECommerceAPI.API\\wwwroot\\{t.Path}",
            FileName = t.FileName,
            Id = t.Id
        }).ToList();
    }
}