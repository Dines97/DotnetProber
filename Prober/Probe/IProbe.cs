using System.Text.Json;
using System.Text.Json.Nodes;
using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;

namespace Prober.Probe;

public interface IProbe {
  public bool Check(ProbeType probeType);

  void SetParameters(string parameters);

  public IHealthCheck Reconcile();

  public ValidationResult Validate(bool dryRun) => ValidationResult.Success();

  public MutationResult Mutate(bool dryRun) => MutationResult.NoChanges();
}