using Common.Utilities.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;

namespace Common.Utilities.Services.ApplicationTelemetryServices;

public class AzureAppInsightTelemetryService(
    ILogger<AzureAppInsightTelemetryService> logger,
    TelemetryClient telemetryClient) : IApplicationTelemetryService
{
    public void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string>? properties = null)
    {
        try
        {
            if (properties != null)
            {
                var propertiesString = properties.ToCommaSeparatedString();
                message += $", {propertiesString}";
            }

            telemetryClient.TrackTrace(message, severityLevel, properties);

            Log(severityLevel, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            logger.LogError(ex, "An error occured while logging telemetry track trace");
            throw;
        }
    }

    public void TrackException(Exception exception, string? message = null, IDictionary<string, string>? properties = null)
    {
        try
        {
            var exceptionMessage = message ?? exception.Message;
            var exceptionProperties = properties ?? new Dictionary<string, string>();

            var exceptionTelemetry = new ExceptionTelemetry(exception)
            {
                Message = exceptionMessage
            };

            foreach (var kvp in exceptionProperties)
            {
                exceptionTelemetry.Properties[kvp.Key] = kvp.Value;
            }

            telemetryClient.TrackException(exceptionTelemetry);
            
            if (properties != null)
            {
                var propertiesString = properties.ToCommaSeparatedString();
                exceptionMessage += $", {propertiesString}";
            }

            logger.LogError(exception, exceptionMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            logger.LogError(ex, "An error occured while logging telemetry exception trace");
            throw;
        }
    }

    private void Log(SeverityLevel severityLevel, string message, IDictionary<string, string>? properties = null)
    {
        switch (severityLevel)
        {
            case SeverityLevel.Critical:
                logger.LogCritical(message);
                break;
            case SeverityLevel.Error:
                logger.LogError(message);
                break;
            case SeverityLevel.Warning:
                logger.LogWarning(message);
                break;
            case SeverityLevel.Information:
                logger.LogInformation(message);
                break;
            case SeverityLevel.Verbose:
                logger.LogTrace(message);
                break;
            default:
                logger.LogInformation(message);
                break;
        }
    }
}