using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.Dtos;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Dom = ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
{
    private readonly UserManager<Dom.AppUser> _userManager;
    private readonly ITokenHandler _tokenHandler;
    public GoogleLoginCommandHandler(UserManager<Dom.AppUser> userManager, ITokenHandler tokenHandler)
    {
        _userManager = userManager;
        _tokenHandler = tokenHandler;
    }

    public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request,
        CancellationToken cancellationToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { "517814878199-u4ilo7hup1944uosqpbe8phab3h9d5db.apps.googleusercontent.com" }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

        var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);
        Dom.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey) ??
                           await _userManager.FindByEmailAsync(payload.Email);
        bool result = true;
        if (user == null)
        {
            user = new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = payload.Email,
                UserName = payload.Email,
                NameSurname = payload.Name
            };
            var identityResult = await _userManager.CreateAsync(user); 
            result = identityResult.Succeeded;
        }
        if (result)
            await _userManager.AddLoginAsync(user, info); //AspnetuserLogins tablosuna kullanıcı eklenir.
        else
            throw new Exception("Invalid External Authentication!");

        Token token = _tokenHandler.CreateAccessToken(5);
        
        return new()
        {
            Token = token
        };
    }
}