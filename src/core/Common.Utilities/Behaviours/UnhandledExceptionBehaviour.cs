using MediatR;

namespace Common.Utilities.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(
    ICommonUser commonUser,
    IApplicationTelemetryService telemetryService,
    IConfiguration configuration) 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ValidationException)
        {
            //ignore validation exceptions which are handled by ValidationBehaviour
            throw;
        }
        catch (Exception ex)
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

            telemetryService.TrackException(ex, "Unhandled Exception", properties);

            throw;
        }
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
            telemetryService.TrackException(ex, "UnhandledExceptionBehaviour - An error occured while getting user");
        }

        return null;
    }
}