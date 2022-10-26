using ECommerceAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Dom = ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
{
    private readonly UserManager<Dom.AppUser> _userManager;

    public CreateUserCommandHandler(UserManager<Dom.AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request,
        CancellationToken cancellationToken)
    {
        IdentityResult result = await _userManager.CreateAsync(new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.UserName,
            Email = request.Email,
            NameSurname = request.NameSurname,
        }, request.Password);

        CreateUserCommandResponse response = new()
        {
            Succeeded = result.Succeeded
        };

        if (result.Succeeded)
            response.Message = "Kullanıcı başarıyla oluşturuldu...";
        else
            result.Errors.ToList().ForEach(error =>
            {
                response.Message += $"{error.Code} - {error.Description}\n";
            });
        return response;
    }
}