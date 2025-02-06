using FluentValidation;
using MediatR;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;

namespace Common.Utilities.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(
    ICommonUser commonUser,
    IApplicationTelemetryService telemetryService,
    IConfiguration configuration,
    IEnumerable<IValidator<TRequest>> validators) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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

        telemetryService.TrackTrace($"Request Validation Started: {requestName}", SeverityLevel.Information, properties);

        var validatorsList = validators.ToList();
        if (validatorsList.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validatorsList.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(x => x.Errors.Any())
                .SelectMany(x => x.Errors)
                .ToList();

            if (failures.Any())
            {
                var failureMessages = JsonConvert.SerializeObject(failures);
                telemetryService.TrackTrace($"Request Validation Failures: {failureMessages}", SeverityLevel.Warning, properties);

                telemetryService.TrackTrace($"Request Validation Completed: {requestName}", SeverityLevel.Information, properties);
                throw new Exceptions.ValidationException(failures);
            }
        }

        telemetryService.TrackTrace($"Request Validation Completed: {requestName}", SeverityLevel.Information, properties);

        return await next();
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
            telemetryService.TrackException(ex, "ValidationBehaviour - An error occured while getting user");
        }

        return null;
    }
}