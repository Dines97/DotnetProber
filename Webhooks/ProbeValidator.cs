using KubeOps.Operator.Webhooks;
using Prober.Entities;

namespace Prober.Webhooks;

public class ProbeValidator : IValidationWebhook<V1Alpha1ProbeEntity> {
  public AdmissionOperations Operations => AdmissionOperations.Create;

  public ValidationResult Create(V1Alpha1ProbeEntity entity, bool dryRun) {
    return ValidationResult.Success();
  }
}