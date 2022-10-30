using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Dom = ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
{
    private readonly UserManager<Dom.AppUser> _userManager;
    private readonly SignInManager<Dom.AppUser> _signInManager;
    private readonly ITokenHandler _tokenHandler;

    public LoginUserCommandHandler(UserManager<Dom.AppUser> userManager, SignInManager<Dom.AppUser> signInManager, ITokenHandler tokenHandler)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenHandler = tokenHandler;
    }

    public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request,
        CancellationToken cancellationToken)
    {
        Dom.AppUser? user = await _userManager.FindByNameAsync(request.UsernameOrEmail) ??
                            await _userManager.FindByEmailAsync(request.UsernameOrEmail);
        if (user == null)
            throw new NotFoundUserException();

        //result.Succeeded true ise authentication basarili.
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);  
        if (result.Succeeded)
        {
            Token token = _tokenHandler.CreateAccessToken(5);
            return new LoginUserSuccessCommandResponse()
            {
                AccessToken = token 
            };
        }

        // return new LoginUserErrorCommandResponse()
        // {
        //     Message = "Kullanıcı adı veya şifre hatalı..."
        // };
        throw new AuthenticationErrorException();
    }
}