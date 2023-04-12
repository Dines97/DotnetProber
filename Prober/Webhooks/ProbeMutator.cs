using KubeOps.Operator.Webhooks;
using Prober.Entities;
using Prober.Probe;

namespace Prober.Webhooks;

public class ProbeMutator : IMutationWebhook<V1Alpha1ProbeEntity> {
  public AdmissionOperations Operations => AdmissionOperations.Create;

  public MutationResult Create(V1Alpha1ProbeEntity newEntity, bool dryRun) {
    var probe = newEntity.Spec.Type.GetProbe(newEntity.Spec.Parameters);
    //var parameters = probe.Deserialize(newEntity.Spec.Parameters);

    return new MutationResult();
  }
}