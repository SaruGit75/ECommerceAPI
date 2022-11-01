namespace ECommerceAPI.Application.Abstractions.Token;

public interface ITokenHandler
{
    Dtos.Token CreateAccessToken(int second);
}