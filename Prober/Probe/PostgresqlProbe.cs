using HealthChecks.NpgSql;
using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;

namespace Prober.Probe;

public class PostgresqlProbe : IProbe {
  private readonly IDeserializer _deserializer;
  private PostgresqlParameters _parameters = null!;

  public PostgresqlProbe(IDeserializer deserializer) {
    _deserializer = deserializer;
  }

  public bool Check(ProbeType probeType) {
    return probeType == ProbeType.Postgresql;
  }

  public void SetParameters(string parameters) {
    _parameters = _deserializer.Deserialize<PostgresqlParameters>(parameters);
  }

  public IHealthCheck Reconcile() {
    return new NpgSqlHealthCheck(_parameters.ConnectionString!, "SELECT 1;");
  }

  public ValidationResult Validate(bool dryRun) {
    return string.IsNullOrEmpty(_parameters.ConnectionString)
      ? ValidationResult.Fail(StatusCodes.Status400BadRequest, "connectionString should be specified")
      : ValidationResult.Success();
  }

  public MutationResult Mutate(bool dryRun) {
    throw new NotImplementedException();
  }
}