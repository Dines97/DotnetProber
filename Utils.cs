using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace Prober;

public static class Utils {
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
}