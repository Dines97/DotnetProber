using k8s.Models;
using KubeOps.Operator.Finalizer;
using Prober.Entities;

namespace Prober.Finalizer;

public class ProbeFinalizer : IResourceFinalizer<V1Alpha1ProbeEntity> {
  private readonly ILogger<ProbeFinalizer> _logger;

  public ProbeFinalizer(ILogger<ProbeFinalizer> logger) {
    _logger = logger;
  }

  public Task FinalizeAsync(V1Alpha1ProbeEntity entity) {
    _logger.LogInformation($"entity {entity.Name()} called {nameof(FinalizeAsync)}.");

    return Task.CompletedTask;
  }
}