using ECommerceAPI.Application.Dtos.User;

namespace ECommerceAPI.Application.Abstractions.Services;

public interface IUserService
{
    Task<CreateUserResponse> CreateAsync(CreateUser model);
}