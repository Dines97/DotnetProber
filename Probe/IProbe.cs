using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;

namespace Prober.Probe;

public interface IProbe { }

public interface IProbe<in T> : IProbe where T : class, IProbeParameters {
  public IHealthCheck Reconcile(T parameters);

  public ValidationResult Validate(T parameters, bool dryRun);

  public MutationResult Mutate(T parameters, bool dryRun);
}