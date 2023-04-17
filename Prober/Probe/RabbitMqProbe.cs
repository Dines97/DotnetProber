using System.Text.Json;
using HealthChecks.RabbitMQ;
using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;

namespace Prober.Probe;

public class RabbitMqProbe : IProbe {
  private readonly IDeserializer _deserializer;
  private RabbitMqParameters _parameters = null!;

  public RabbitMqProbe(IDeserializer deserializer) {
    _deserializer = deserializer;
  }

  public bool Check(ProbeType probeType) {
    return probeType == ProbeType.RabbitMq;
  }

  public void SetParameters(string parameters) {
    _parameters = _deserializer.Deserialize<RabbitMqParameters>(parameters);
  }

  public IHealthCheck Reconcile() {
    return new RabbitMQHealthCheck(new Uri(_parameters.ConnectionString!), null);
  }

  public ValidationResult Validate(bool dryRun) {
    return string.IsNullOrEmpty(_parameters.ConnectionString)
      ? ValidationResult.Fail(StatusCodes.Status400BadRequest, "connectionString should be specified")
      : ValidationResult.Success();
  }
}