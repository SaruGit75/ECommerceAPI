namespace ECommerceAPI.Application.Abstractions.Services.Authentication;

public interface IExternalAuthentication
{
    Task<Dtos.Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime);
}