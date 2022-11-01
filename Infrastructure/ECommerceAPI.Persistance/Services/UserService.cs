using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Dtos.User;
using Microsoft.AspNetCore.Identity;
using Dom = ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Persistance.Services;

public class UserService : IUserService
{
    private readonly UserManager<Dom.AppUser> _userManager;

    public UserService(UserManager<Dom.AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CreateUserResponse> CreateAsync(CreateUser model)
    {
        IdentityResult result = await _userManager.CreateAsync(new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            Email = model.Email,
            NameSurname = model.NameSurname,
        }, model.Password);

        CreateUserResponse response = new()
        {
            Succeeded = result.Succeeded
        };

        if (result.Succeeded)
            response.Message = "Kullanıcı başarıyla oluşturuldu...";
        else
            result.Errors.ToList().ForEach(error => { response.Message += $"{error.Code} - {error.Description}\n"; });

        return response;
    }
}