# Common.Utilities

### 06/02/2025 - Version 1.0.1
- Added Logger, Azure AppInsights Telemetry, 
- Added Request Behaviours for MediatR Pipelines (`LoggingBehaviour`, `UnhandledExceptionBehaviour`, `ValidationBehaviour`, `PerformanceBehaviour`, `RequestPostProcessBehaviour`)
- Added `ValidationException` exception
- Added Dictionary Extensions
- Added `ConfigureServices` class for Dependency Injecttion with `AddCommonServices` method
- Created a simple interface (`IApplicationTelemetryService`) with implementation to log and adding telemtry for Azure App Insights
- Created `ICommonsUser` interface with a method `GetUser` that returns basic user information

### 05/02/2025 - Version 1.0.0
- Initial release of Common.Utilities
- Project structure is created
- Added Logger, AppInsights, AppSettings, AppMetrics, AppPerformance, AppException, AppResponse, AppRequest