using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Milda.B2C.Function.Services;

namespace Milda.B2C.Function.Web;

public class UserFunctions(IAuthorizationService authorizationService) : MildaFunction(authorizationService)
{
    [Function("GetUserData")]
    [OpenApiOperation(operationId: "GetUserData", tags: new[] { "user" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<string>), Description = "The list of audits")]
    public async Task<HttpResponseData> GetUserData(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var authResult = await Authorise(req);
        if (!authResult.Success)
        {
            return authResult.Result;
        }

        // Authorization successful
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync($"Token validated successfully! Name: {DisplayName}, Email: {Email}, Id: {UserId.Substring(0,4)}...");
        return response;
    }
}