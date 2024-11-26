using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace Milda.B2C.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options =>
                    {
                        builder.Configuration.Bind("AzureAdB2C", options);

                        var validIssuers = builder.Configuration
                            .GetSection("AzureAdB2C:ValidIssuers")
                            .Get<string[]>();

                        var validAudiences = builder.Configuration
                            .GetSection("AzureAdB2C:ValidAudiences")
                            .Get<string[]>();

                        options.TokenValidationParameters.ValidIssuers = validIssuers;
                        options.TokenValidationParameters.ValidAudiences = validAudiences;

                        // Debugging Events
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                Console.WriteLine("Token successfully validated.");
                                return Task.CompletedTask;
                            }
                        };
                    },
                    options => builder.Configuration.Bind("AzureAdB2C", options));



            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void AuthOptions(JwtBearerOptions options)
        {
            
        }
    }
}
