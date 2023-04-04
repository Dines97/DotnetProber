using KubeOps.Operator.Webhooks;
using Prober.Entities;

namespace Prober.Webhooks;

public class DemoMutator : IMutationWebhook<V1DemoEntity>
{
    public AdmissionOperations Operations => AdmissionOperations.Create;

    public MutationResult Create(V1DemoEntity newEntity, bool dryRun)
    {
        newEntity.Spec.Username = "not foobar";
        return MutationResult.Modified(newEntity);
    }
}
