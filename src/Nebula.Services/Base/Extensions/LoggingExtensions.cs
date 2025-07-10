using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownService";
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(
                        ResourceBuilder.CreateDefault().AddService(serviceName)
                    );

                    options.IncludeScopes = true;
                    options.IncludeFormattedMessage = true;
                    options.ParseStateValues = true;

                    // Exporter: console by default
                    options.AddConsoleExporter();
                });
            });

            return services;
        }
    }
}
