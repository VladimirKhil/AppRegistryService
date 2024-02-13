using System.Diagnostics.Metrics;

namespace AppRegistryService.Metrics;

/// <summary>
/// Holds service metrics.
/// </summary>
public sealed class OtelMetrics
{
    private Counter<int> AppRunsCounter { get; }

    private Counter<int> AppErrorsCounter { get; }

    public string MeterName { get; }

    public OtelMetrics(string meterName = "AppRegistry")
    {
        var meter = new Meter(meterName);
        MeterName = meterName;

        AppRunsCounter = meter.CreateCounter<int>("app-runs");
        AppErrorsCounter = meter.CreateCounter<int>("app-errors");
    }

    public void AddRun(Guid appId) => AppRunsCounter.Add(
        1,
        new KeyValuePair<string, object?>("appId", appId));

    public void AddError(Guid appId) => AppErrorsCounter.Add(
        1,
        new KeyValuePair<string, object?>("appId", appId));
}
