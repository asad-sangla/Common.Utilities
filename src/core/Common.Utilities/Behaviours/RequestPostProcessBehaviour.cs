using MediatR;
using Microsoft.ApplicationInsights.DataContracts;

namespace Common.Utilities.Behaviours;

public class RequestPostProcessBehaviour<TRequest, TResponse>(
    ICommonUser commonUser,
    IApplicationTelemetryService telemetryService,
    IConfiguration configuration)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

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

        telemetryService.TrackTrace("Request Execution Completed", SeverityLevel.Information, properties);

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
            telemetryService.TrackException(ex, "RequestPostProcessBehaviour - An error occured while getting user");
        }

        return null;
    }
}