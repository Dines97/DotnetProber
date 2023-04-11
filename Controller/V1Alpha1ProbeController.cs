using System.Globalization;
using k8s.Models;
using KubeOps.KubernetesClient;
using KubeOps.Operator.Controller;
using KubeOps.Operator.Controller.Results;
using KubeOps.Operator.Finalizer;
using KubeOps.Operator.Kubernetes;
using KubeOps.Operator.Rbac;
using Prober.Entities;
using Prober.Finalizer;
using Prober.Probe;
using Prober.ProbeManager;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Prober.Controller;

[EntityRbac(typeof(V1Alpha1ProbeEntity), Verbs = RbacVerb.All)]
public class V1Alpha1ProbeController : IResourceController<V1Alpha1ProbeEntity> {
  private readonly ILogger<V1Alpha1ProbeController> _logger;
  private readonly IFinalizerManager<V1Alpha1ProbeEntity> _finalizerManager;
  private readonly IKubernetesClient _client;
  private readonly IProbeManager _probeManager;

  private readonly IDeserializer _deserializer =
    new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

  public V1Alpha1ProbeController(ILogger<V1Alpha1ProbeController> logger,
    IFinalizerManager<V1Alpha1ProbeEntity> finalizerManager,
    IKubernetesClient client, IProbeManager probeManager) {
    _logger = logger;
    _finalizerManager = finalizerManager;
    _client = client;
    _probeManager = probeManager;
  }


  public async Task<ResourceControllerResult?> ReconcileAsync(V1Alpha1ProbeEntity entity) {
    _logger.LogInformation("entity {} called {}", entity.Name(), nameof(ReconcileAsync));
    // await _finalizerManager.RegisterFinalizerAsync<ProbeFinalizer>(entity);

    await _probeManager.ReconciledAsync(entity);

    await _client.UpdateStatus(entity);

    // HACK: Need to sleep after UpdateStatus or Reconcile will not be requeued
    Thread.Sleep(1000);

    return ResourceControllerResult.RequeueEvent(TimeSpan.FromSeconds(10), ResourceEventType.Reconcile);
  }

  public Task StatusModifiedAsync(V1Alpha1ProbeEntity entity) {
    return Task.CompletedTask;
  }
}