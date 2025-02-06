using MediatR;
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;

namespace Common.Utilities.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(
    ICommonUser commonUser,
    IApplicationTelemetryService telemetryService,
    IConfiguration configuration) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        var thresholdInSecondsString = configuration["AppRequestPerformanceThresholdInSeconds"] ?? string.Empty;
        var thresholdInSeconds = !string.IsNullOrEmpty(thresholdInSecondsString) ? Convert.ToInt32(thresholdInSecondsString) : 2;
        var thresholdInMilliSeconds = thresholdInSeconds * 1000;

        if (elapsedMilliseconds <= thresholdInMilliSeconds) 
            return response;

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

        telemetryService.TrackTrace($"Long Running Request Detected, Time elapsed: {elapsedMilliseconds} ms", SeverityLevel.Warning, properties);

        return response;
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
            telemetryService.TrackException(ex, "PerformanceBehaviour - An error occured while getting user");
        }

        return null;
    }
}