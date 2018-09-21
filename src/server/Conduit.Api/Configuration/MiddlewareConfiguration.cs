using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Conduit.Api.Configuration
{
    public static class MiddlewareConfiguration
    {
        public static void UseSwagger(this IApplicationBuilder app, string endpointName)
        {
            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                setup.RoutePrefix = string.Empty;

                setup.SwaggerEndpoint(
                    url: "/swagger/v1/swagger.json",
                    name: endpointName);
            });
        }

        public static void AddLogging(this ILoggerFactory loggerFactory, IConfigurationSection loggingConfiguration)
        {
            loggerFactory.AddConsole(loggingConfiguration);
            loggerFactory.AddFile("logs/web-api-{Date}.txt");
            loggerFactory.AddDebug();
        }
    }
}
