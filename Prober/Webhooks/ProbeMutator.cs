using KubeOps.Operator.Webhooks;
using Prober.Entities;
using Prober.Probe;

namespace Prober.Webhooks;

public class ProbeMutator : IMutationWebhook<V1Alpha1ProbeEntity> {
  private readonly ILogger<ProbeMutator> _logger;

  private readonly IEnumerable<IProbe> _probes;

  public ProbeMutator(IEnumerable<IProbe> probes, ILogger<ProbeMutator> logger) {
    _probes = probes;
    _logger = logger;
  }

  public AdmissionOperations Operations => AdmissionOperations.All;


  public MutationResult Create(V1Alpha1ProbeEntity newEntity, bool dryRun) {
    return All(newEntity, dryRun);
  }

  public MutationResult Update(V1Alpha1ProbeEntity oldEntity, V1Alpha1ProbeEntity newEntity, bool dryRun) {
    return All(newEntity, dryRun);
  }

  public MutationResult Delete(V1Alpha1ProbeEntity oldEntity, bool dryRun) {
    return MutationResult.NoChanges();
  }

  public MutationResult All(V1Alpha1ProbeEntity newEntity, bool dryRun) {
    var probe = _probes.SingleOrDefault(x => x.Check(newEntity.Spec.Type));
    if (probe == null) {
      _logger.LogInformation("Doesn't found probe that satisfies {}", newEntity.Spec.Type);
      return new MutationResult {
        Valid = false,
        StatusCode = 400,
        StatusMessage = $"Doesn't found probe that satisfies {newEntity.Spec.Type}"
      };
    }

    // probe.SetParameters(newEntity.Spec.Parameters);
    // return probe.Mutate(dryRun);
    return MutationResult.Modified(newEntity);
  }
}