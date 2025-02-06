using Microsoft.ApplicationInsights.DataContracts;

namespace Common.Utilities.Interfaces;

public interface IApplicationTelemetryService
{
    void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string>? properties = null);
    void TrackException(Exception exception, string? message = null, IDictionary<string, string>? properties = null);
    //void TrackEvent(string eventName, IDictionary<string, string>? properties = null);
    //void TrackMetric(string name, double value, IDictionary<string, string>? properties = null);
}