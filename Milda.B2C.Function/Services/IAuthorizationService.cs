using System.Security.Claims;

namespace Milda.B2C.Function.Services;

public interface IAuthorizationService
{
    ClaimsPrincipal? ValidateToken(string token);
    bool ValidateApiKey(string apiKey);
}