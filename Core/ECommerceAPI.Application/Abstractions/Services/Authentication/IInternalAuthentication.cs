namespace ECommerceAPI.Application.Abstractions.Services.Authentication;

public interface IInternalAuthentication
{
    Task<Dtos.Token> LoginAsync(string userNameOrEmail, string password, int accessTokenLifeTime);
}