using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;

namespace Prober.Probe; 

public class DnsProbe : IProbe {
  public bool Check(ProbeType probeType) {
    return probeType == ProbeType.DnsProbe;
  }

  public void SetParameters(string parameters) {
    throw new NotImplementedException();
  }

  public IHealthCheck Reconcile() {
    throw new NotImplementedException();
  }

  public ValidationResult Validate(bool dryRun) {
    throw new NotImplementedException();
  }

  public MutationResult Mutate(bool dryRun) {
    throw new NotImplementedException();
  }
}