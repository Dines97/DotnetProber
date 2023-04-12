using HealthChecks.NpgSql;
using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;

namespace Prober.Probe;

public class PostgresqlProbe : IProbe<IProbeParameters> {
  public IHealthCheck Reconcile(IProbeParameters parameters) {
    var p = (PostgresqlParameters)parameters;

    return new NpgSqlHealthCheck(p.ConnectionString!, "SELECT 1;");
  }

  public ValidationResult Validate(IProbeParameters parameters, bool dryRun) {
    var p = (PostgresqlParameters)parameters;
    return string.IsNullOrEmpty(p.ConnectionString)
      ? ValidationResult.Fail(StatusCodes.Status400BadRequest, "connectionString should be specified")
      : ValidationResult.Success();
  }

  public MutationResult Mutate(IProbeParameters parameters, bool dryRun) {
    throw new NotImplementedException();
  }
}