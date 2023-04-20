using KubeOps.Operator.Webhooks;
using Prober.Entities;

namespace Prober.Webhooks;

public class ProbeValidator : IValidationWebhook<V1Alpha1ProbeEntity> {
  public AdmissionOperations Operations => AdmissionOperations.All;

  public ValidationResult Create(V1Alpha1ProbeEntity entity, bool dryRun) {
    return ValidationResult.Success();
  }

  public ValidationResult Update(V1Alpha1ProbeEntity oldEntity, V1Alpha1ProbeEntity newEntity, bool dryRun) {
    return ValidationResult.Success();
  }

  public ValidationResult Delete(V1Alpha1ProbeEntity oldEntity, bool dryRun) {
    return ValidationResult.Success();
  }
}