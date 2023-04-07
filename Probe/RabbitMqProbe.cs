using System.Runtime.Serialization;
using System.Text.Json;
using HealthChecks.RabbitMQ;
using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Prober.Probe;

public class RabbitMqProbe : IProbe<IProbeParameters> {
  public IHealthCheck Reconcile(IProbeParameters parameters) {
    var p = (RabbitMqParameters)parameters;

    return new RabbitMQHealthCheck(new Uri(p.ConnectionString!), null);
  }

  public ValidationResult Validate(IProbeParameters parameters, bool dryRun) {
    var p = (RabbitMqParameters)parameters;
    return string.IsNullOrEmpty(p.ConnectionString)
      ? ValidationResult.Fail(StatusCodes.Status400BadRequest, "connectionString should be specified")
      : ValidationResult.Success();
  }

  public MutationResult Mutate(IProbeParameters parameters, bool dryRun) {
    throw new NotImplementedException();
  }
}