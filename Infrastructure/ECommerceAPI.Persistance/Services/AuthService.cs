using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Dom = ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Persistance.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<Dom.AppUser> _userManager;
    private readonly ITokenHandler _tokenHandler;
    private readonly SignInManager<Dom.AppUser> _signInManager;

    public AuthService(IConfiguration configuration, UserManager<Dom.AppUser> userManager, ITokenHandler tokenHandler, SignInManager<Dom.AppUser> signInManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        _signInManager = signInManager;
    }

    public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { _configuration["ExternalLoginSettings:Google:Client_Id"] }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");
        Dom.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey) ??
                           await _userManager.FindByEmailAsync(payload.Email);
        
        return await _CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifeTime);
    }

    public async Task<Token> LoginAsync(string userNameOrEmail, string password, int accessTokenLifeTime)
    {
        Dom.AppUser? user = await _userManager.FindByNameAsync(userNameOrEmail) ??
                            await _userManager.FindByEmailAsync(userNameOrEmail);
        if (user == null)
            throw new NotFoundUserException();

        //result.Succeeded true ise authentication basarili.
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);  
        if (result.Succeeded)
        {
            Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
            return token;
        }
        throw new AuthenticationErrorException();
    }

    private async Task<Token> _CreateUserExternalAsync(Dom.AppUser? user, string email, string name,
        UserLoginInfo userLoginInfo, int accessTokenLifeTime)
    {
        var result = user != null;
        if (user == null)
        {
            user = new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = email,
                NameSurname = name
            };
            var identityResult = await _userManager.CreateAsync(user);
            result = identityResult.Succeeded;
        }

        if (result)
        {
            await _userManager.AddLoginAsync(user, userLoginInfo); //AspnetuserLogins tablosuna kullanıcı eklenir.
            Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
            return token;
        }

        throw new Exception("Invalid External Authentication!");
    }
}