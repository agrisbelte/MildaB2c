using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Swashbuckle.AspNetCore.Swagger;

namespace Milda.B2C.Function.Web;

public class SwaggerFunctions
{
    private readonly ISwaggerProvider _swaggerProvider;

    public SwaggerFunctions(ISwaggerProvider swaggerProvider)
    {
        _swaggerProvider = swaggerProvider;
    }

    [Function("SwaggerJson")]
    public async Task<HttpResponseData> GetSwaggerJson(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/v1/swagger.json")] HttpRequestData req)
    {
        var swagger = _swaggerProvider.GetSwagger("v1"); // Generate the Swagger document
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        // Serialize the Swagger document to JSON
        var swaggerJson = JsonSerializer.Serialize(swagger, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteStringAsync(swaggerJson);
        return response;
    }

    [Function("SwaggerUI")]
    public async Task<HttpResponseData> GetSwaggerUI(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/ui")] HttpRequestData req)
    {
        var swaggerUI = SwaggerUIBuilder.Build("http://localhost:7034/api/swagger/v1/swagger.json");
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/html");
        await response.WriteStringAsync(swaggerUI);
        return response;
    }
}