using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Milda.B2C.Function.Services;
using Microsoft.OpenApi.Models;
using Milda.B2C.Function.Models;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
#if DEBUG
        config.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
#else
        config.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);
#endif
        config.AddEnvironmentVariables(); // Still support environment variables
    })
    .ConfigureServices((context, services) => {

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Mandalog function API",
                Version = "v1",
                Description = "Azure Functions Isolated Process API with Swagger"
            });
        });

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        services.AddSingleton<TokenValidationParameters>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();

            var azureAdB2CConfig = context.Configuration.GetSection("AzureAdB2C").Get<AzureAdB2COptions>();

            if (azureAdB2CConfig == null)
            {
                throw new InvalidOperationException("AzureAdB2C configuration is missing.");
            }

            return new TokenValidationParameters
            {
                // Required: Valid Issuers
                ValidIssuers = azureAdB2CConfig.ValidIssuers,

                // Required: Valid Audiences
                ValidAudiences = azureAdB2CConfig.ValidAudiences,

                // Signing Key Resolver: Retrieves signing keys dynamically from B2C OpenID Connect metadata
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    var metadataAddress = $"{azureAdB2CConfig.Instance.TrimEnd('/')}/{azureAdB2CConfig.Domain.TrimEnd('/')}/v2.0/.well-known/openid-configuration?p={azureAdB2CConfig.SignUpSignInPolicyId}";
                    var configurationManager = new Microsoft.IdentityModel.Protocols.ConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>(
                        metadataAddress,
                        new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever());
                    var openIdConfig = configurationManager.GetConfigurationAsync(default).Result;
                    
                    return openIdConfig.SigningKeys;
                },

                // Optional: Validate Issuer
                ValidateIssuer = true,

                // Optional: Validate Audience
                ValidateAudience = true,

                // Optional: Validate Lifetime
                ValidateLifetime = true,

                // Optional: Validate Signing Key
                ValidateIssuerSigningKey = true,

                // Optional: Clock Skew
                ClockSkew = TimeSpan.FromMinutes(5) // Adjust for clock skew between systems
            };
        });

        // Add AuthorizationService
        services.AddSingleton<IAuthorizationService>(provider =>
        {
            var tokenValidationParameters = provider.GetRequiredService<TokenValidationParameters>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var apiKey = configuration["ApiKey"];
            return new AuthorizationService(tokenValidationParameters, apiKey);
        });

    }).ConfigureLogging(logging =>
    {
        // Disable IHttpClientFactory Informational logs.
        // Note -- you can also remove the handler that does the logging: https://github.com/aspnet/HttpClientFactory/issues/196#issuecomment-432755765 
        logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
    })
    .Build();

host.Run();
