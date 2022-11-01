using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Dtos.User;
using ECommerceAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Dom = ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request,
        CancellationToken cancellationToken)
    {
        CreateUserResponse response =  await _userService.CreateAsync(new()
        {
            Email = request.Email,
            Password = request.Password,
            NameSurname = request.NameSurname,
            PasswordConfirm = request.PasswordConfirm,
            UserName = request.UserName
        });
        
        return new ()
        {
            Message = response.Message,
            Succeeded = response.Succeeded
        };
    }
}