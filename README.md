# Common.Utilities
This Repository contains common utilities which can be used in any .Net Core application.

## Instructions to use this package
- To user Mediator Pipeline Behaviours, you need to add the following behaviours (`LoggingBehaviour`, `UnhandledExceptionBehaviour`, `ValidationBehaviour`, `PerformanceBehaviour`, `RequestPostProcessBehaviour`)
- It also uses Azure Application Insights to log the telemetry data, please follow configuration settings instructions
- Mediator Pipeline Behaviours require user information to log the telemetry data, please implement the `ICommonUser` interface that returns the infomation of logged in user

## Add Following configuration settings in your appsettings.json file

```json
{
"ApplicationName": "[YOUR APPLICATION NAME]",
"AppRequestPerformanceThresholdInSeconds": "[ADD YOURS] default value is 2",
  "ApplicationInsights": {
	"ConnectionString": "[AAZURE APPLICATION INSIGHTS CONNECTION STRING]"
  }
}
```