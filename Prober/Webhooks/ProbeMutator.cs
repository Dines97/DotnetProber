using KubeOps.Operator.Webhooks;
using Prober.Entities;
using Prober.Probe;

namespace Prober.Webhooks;

public class ProbeMutator : IMutationWebhook<V1Alpha1ProbeEntity> {
  public AdmissionOperations Operations => AdmissionOperations.Create | AdmissionOperations.Update;

  private readonly IEnumerable<IProbe> _probes;
  private readonly ILogger<ProbeMutator> _logger;

  public ProbeMutator(IEnumerable<IProbe> probes, ILogger<ProbeMutator> logger) {
    _probes = probes;
    _logger = logger;
  }

  public MutationResult All(V1Alpha1ProbeEntity newEntity, bool dryRun) {
    var probe = _probes.SingleOrDefault(x => x.Check(newEntity.Spec.Type));
    if (probe == null) {
      _logger.LogInformation("Doesn't found probe that satisfies {}", newEntity.Spec.Type);
      return new MutationResult() {
        Valid = false,
        StatusCode = 400,
        StatusMessage = $"Doesn't found probe that satisfies {newEntity.Spec.Type}"
      };
    }

    probe.SetParameters(newEntity.Spec.Parameters);
    // return probe.Mutate(dryRun);
    return MutationResult.Modified(newEntity);
  }


  public MutationResult Create(V1Alpha1ProbeEntity newEntity, bool dryRun) {
    return All(newEntity, dryRun);
  }

  public MutationResult Update(V1Alpha1ProbeEntity oldEntity, V1Alpha1ProbeEntity newEntity, bool dryRun) {
    return All(newEntity, dryRun);
  }
}