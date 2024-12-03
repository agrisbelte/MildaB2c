using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;
using Milda.B2C.Function.Services;

namespace Milda.B2C.Function.Web;

public class MildaFunction(IAuthorizationService authorizationService)
{
    protected IAuthorizationService AuthorizationService { get; } = authorizationService;
    protected List<string>? Roles { get; private set; }
    protected string? Email { get; private set; }
    protected string? UserId { get; private set; }
    protected string? DisplayName { get; private set; }

    protected async Task<(bool Success, HttpResponseData Result)> Authorise(HttpRequestData req)
    {
        // Validate API Key
        if (!req.Headers.TryGetValues("x-api-key", out var apiKeyValues) ||
            !AuthorizationService.ValidateApiKey(apiKeyValues.First()))
        {
            return await CreateErrorResponse(req, HttpStatusCode.Unauthorized, "Invalid API Key.");
        }

        // Validate Bearer Token
        if (!req.Headers.TryGetValues("Authorization", out var authHeaderValues) ||
            !authHeaderValues.First().StartsWith("Bearer "))
        {
            return await CreateErrorResponse(req, HttpStatusCode.Unauthorized, "Missing or invalid Authorization header.");
        }

        var token = authHeaderValues.First()["Bearer ".Length..];
        var principal = AuthorizationService.ValidateToken(token);

        if (principal == null)
        {
            return await CreateErrorResponse(req, HttpStatusCode.Forbidden, "Invalid or expired token.");
        }

        SetUserDetails(principal);

        // Authorization successful
        return (Success: true, Result: null);
    }

    private async Task<(bool Success, HttpResponseData Result)> CreateErrorResponse(
        HttpRequestData req,
        HttpStatusCode statusCode,
        string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteStringAsync(message);
        return (Success: false, Result: response);
    }

    private void SetUserDetails(ClaimsPrincipal? principal)
    {
        if (principal == null)
        {
            return;
        }

        // Extract and save claims
        UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        DisplayName = principal.FindFirst("name")?.Value;
        Email = principal.FindFirst("emails")?.Value;
        Roles = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();
    }
}