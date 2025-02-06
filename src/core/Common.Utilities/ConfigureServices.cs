using Common.Utilities.Services.ApplicationTelemetryServices;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Utilities;

public static class ConfigureServices
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureAzureAppInsight(services, configuration);

        services.AddScoped<IApplicationTelemetryService, AzureAppInsightTelemetryService>();

        return services;
    }

    private static void ConfigureAzureAppInsight(IServiceCollection services, IConfiguration configuration)
    {
        var appInsightsConnectionString = configuration["ApplicationInsights:ConnectionString"];
        if (!string.IsNullOrEmpty(appInsightsConnectionString))
        {
            services.AddSingleton<TelemetryConfiguration>(provider =>
            {
                var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
                telemetryConfiguration.ConnectionString = appInsightsConnectionString;
                return telemetryConfiguration;
            });

            //register telemetry client
            services.AddSingleton<TelemetryClient>(provider =>
            {
                var telemetryConfiguration = provider.GetRequiredService<TelemetryConfiguration>();
                return new TelemetryClient(telemetryConfiguration);
            });
        }
        else
        {
            Console.WriteLine("ApplicationInsights:ConnectionString is missing in app configurations");
        }
    }
}