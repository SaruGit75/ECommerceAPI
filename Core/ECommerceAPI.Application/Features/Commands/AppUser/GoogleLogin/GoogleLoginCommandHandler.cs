using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Dtos;
using MediatR;
using Dom = ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
{
    private readonly IAuthService _authService;

    public GoogleLoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request,
        CancellationToken cancellationToken)
    {
        Token token = await _authService.GoogleLoginAsync(request.IdToken, 15);
        return new()
        {
            Token = token
        };
    }
}