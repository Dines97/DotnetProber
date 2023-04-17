using System.Text.RegularExpressions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace Prober;

public static class Utils {
  private static readonly IDictionary<string, Func<int, TimeSpan>?> quantityMap =
    new Dictionary<string, Func<int, TimeSpan>?>() {
      { "s", x => TimeSpan.FromSeconds(x) },
      { "m", x => TimeSpan.FromMinutes(x) }
    };

  private static readonly Regex timeQuantityRegex = new(@"^(\d+)(\D+)$", RegexOptions.Compiled);


  public static HealthCheckContext MockHealthCheckContext(IHealthCheck instance) {
    var healthCheckContext = new HealthCheckContext() {
      Registration =
        new HealthCheckRegistration(name: string.Empty,
          instance: instance,
          failureStatus: HealthStatus.Degraded,
          tags: Array.Empty<string>())
    };

    return healthCheckContext;
  }


  public static TimeSpan ParseTimeSpan(string value) {
    var match = timeQuantityRegex.Match(value);

    quantityMap.TryGetValue(match.Groups[2].Value, out var timeSpanFunction);

    return timeSpanFunction?.Invoke(int.Parse(match.Groups[1].Value)) ?? TimeSpan.FromSeconds(30);
  }
}