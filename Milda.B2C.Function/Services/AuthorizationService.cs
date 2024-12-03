using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Milda.B2C.Function.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly string _validApiKey;

    public AuthorizationService(TokenValidationParameters tokenValidationParameters, string validApiKey)
    {
        _tokenValidationParameters = tokenValidationParameters;
        _validApiKey = validApiKey;
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            return handler.ValidateToken(token, _tokenValidationParameters, out _);
        }
        catch (Exception exception)
        {
            var i = exception;
            return null;
        }
    }

    public bool ValidateApiKey(string apiKey)
    {
        return apiKey == _validApiKey;
    }
}