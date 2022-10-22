using ECommerceAPI.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using P = ECommerceAPI.Domain.Entities;
namespace ECommerceAPI.Application.Features.Commands.ProductImageFile.DeleteProductImage;

public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommandRequest, DeleteProductImageCommandResponse>
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly IProductWriteRepository _productWriteRepository;

    public DeleteProductImageCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
    {
        _productReadRepository = productReadRepository;
        _productWriteRepository = productWriteRepository;
    }

    public async Task<DeleteProductImageCommandResponse> Handle(DeleteProductImageCommandRequest request, CancellationToken cancellationToken)
    {
        P.Product? product = await _productReadRepository.Table
            .Include(t => t.ProductImageFiles)
            .FirstOrDefaultAsync(t => t.Id == Guid.Parse(request.Id));

        P.ProductImageFile? productImageFile = product?.ProductImageFiles.FirstOrDefault(t => t.Id == Guid.Parse(request.ImageId));

        if (productImageFile != null) product?.ProductImageFiles.Remove(productImageFile);
        
        await _productWriteRepository.SaveAsync();
        
        return new ();
    }
}