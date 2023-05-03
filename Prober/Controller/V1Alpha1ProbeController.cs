using k8s.Models;
using KubeOps.KubernetesClient;
using KubeOps.Operator.Controller;
using KubeOps.Operator.Controller.Results;
using KubeOps.Operator.Events;
using KubeOps.Operator.Finalizer;
using KubeOps.Operator.Kubernetes;
using KubeOps.Operator.Leadership;
using KubeOps.Operator.Rbac;
using Prober.Entities;
using Prober.ProbeManager;

namespace Prober.Controller;

[EntityRbac(typeof(V1Alpha1ProbeEntity), Verbs = RbacVerb.All)]
public class V1Alpha1ProbeController : IResourceController<V1Alpha1ProbeEntity> {
  private static LeaderState _leaderState;
  private readonly IKubernetesClient _client;
  private readonly IEventManager _eventManager;

  private readonly IFinalizerManager<V1Alpha1ProbeEntity> _finalizerManager;
  private readonly ILogger<V1Alpha1ProbeController> _logger;
  private readonly IProbeManager _probeManager;

  // private static readonly Regex EventRegex =
  //   new Regex(@"Node:\s*([^\s|]*)\s*\|\s*Status:\s*([^\s|]*)\s*", RegexOptions.Compiled);

  public V1Alpha1ProbeController(ILogger<V1Alpha1ProbeController> logger,
    IFinalizerManager<V1Alpha1ProbeEntity> finalizerManager,
    IKubernetesClient client, IProbeManager probeManager, IEventManager eventManager, ILeaderElection leaderElection) {
    _logger = logger;
    _finalizerManager = finalizerManager;
    _client = client;
    _probeManager = probeManager;
    _eventManager = eventManager;

    leaderElection.LeadershipChange.Subscribe(x => _leaderState = x);
  }


  public async Task<ResourceControllerResult?> ReconcileAsync(V1Alpha1ProbeEntity entity) {
    _logger.LogInformation("entity {} called {}", entity.Name(), nameof(ReconcileAsync));
    // await _finalizerManager.RegisterFinalizerAsync<ProbeFinalizer>(entity);

    await _probeManager.ReconciledAsync(entity);

    _logger.LogInformation("Leader state {}", _leaderState);

    if (_leaderState == LeaderState.Leader) {
      entity.Status.Status = await GetStatus(entity);
      await _client.UpdateStatus(entity);
    }

    // HACK: Need to sleep after UpdateStatus or Reconcile will not be requeued
    Thread.Sleep(1000);


    return ResourceControllerResult.RequeueEvent(Utils.Utils.ParseTimeSpan(entity.Spec.Period),
      ResourceEventType.Reconcile);
  }

  public Task StatusModifiedAsync(V1Alpha1ProbeEntity entity) {
    return Task.CompletedTask;
  }


  private async Task<string> GetStatus(V1Alpha1ProbeEntity entity) {
    var resourceNamespace = entity.Namespace() ?? "default";

    var events = await _client.List<Corev1Event>(resourceNamespace);

    var relatedEvents = events
      .Where(x => x.InvolvedObject.Uid == entity.Uid() &&
                  x.Metadata.Annotations.ContainsKey("d-teknoloji.com.tr.prober/node"))
      .GroupBy(x => x.Metadata.Annotations["d-teknoloji.com.tr.prober/node"])
      .Select(x => x.MaxBy(e => e.LastTimestamp)).ToList();

    var healthy = relatedEvents.Count(x => x != null &&
                                           x.Metadata.Annotations.ContainsKey("d-teknoloji.com.tr.prober/status") &&
                                           x.Metadata.Annotations["d-teknoloji.com.tr.prober/status"] == "Healthy");
    var all = relatedEvents.Count;

    return $"{healthy}/{all}";
  }
}