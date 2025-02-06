using MediatR.Pipeline;
using Microsoft.ApplicationInsights.DataContracts;

namespace Common.Utilities.Behaviours;

public class LoggingBehaviour<TRequest>(
    ICommonUser commonUser, 
    IApplicationTelemetryService telemetryService,
    IConfiguration configuration)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var user = GetUser();
        var userName = user != null ? user.UserName : string.Empty;

        var properties = new Dictionary<string, string>
        {
            { "RequestName", requestName },
            { "UserName", userName ?? string.Empty }
        };

        var appInsightsConnectionString = configuration["ApplicationName"] ?? string.Empty;
        if (!string.IsNullOrEmpty(appInsightsConnectionString))
        {
            properties.Add("ApplicationName", appInsightsConnectionString);
        }

        await Task.Run(() => telemetryService.TrackTrace($"Request Execution Started", SeverityLevel.Information, properties), cancellationToken);
    }

    private CommonUserModel? GetUser()
    {
        try
        {
            var user = commonUser.GetUser();
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            telemetryService.TrackException(ex, "LoggingBehaviour - An error occured while getting user");
        }

        return null;
    }
}