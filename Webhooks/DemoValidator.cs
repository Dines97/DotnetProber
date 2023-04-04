using KubeOps.Operator.Webhooks;
using Prober.Entities;

namespace Prober.Webhooks;

public class DemoValidator : IValidationWebhook<V1DemoEntity>
{
    public AdmissionOperations Operations => AdmissionOperations.Create;

    public ValidationResult Create(V1DemoEntity newEntity, bool dryRun)
        => newEntity.Spec.Username == "forbiddenUsername"
            ? ValidationResult.Fail(StatusCodes.Status400BadRequest, "Username is forbidden")
            : ValidationResult.Success();
}
