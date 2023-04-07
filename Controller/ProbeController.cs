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
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Prober.Controller;

[EntityRbac(typeof(V1Alpha1ProbeEntity), Verbs = RbacVerb.All)]
public class ProbeController : IResourceController<V1Alpha1ProbeEntity> {
  private readonly ILogger<ProbeController> _logger;
  private readonly IFinalizerManager<V1Alpha1ProbeEntity> _finalizerManager;
  private readonly IKubernetesClient _client;

  private readonly IDeserializer _deserializer =
    new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();


  public ProbeController(ILogger<ProbeController> logger, IFinalizerManager<V1Alpha1ProbeEntity> finalizerManager,
    IKubernetesClient client) {
    _logger = logger;
    _finalizerManager = finalizerManager;
    _client = client;
  }


  public async Task<ResourceControllerResult?> ReconcileAsync(V1Alpha1ProbeEntity entity) {
    _logger.LogInformation("entity {} called {}", entity.Name(), nameof(ReconcileAsync));
    // await _finalizerManager.RegisterFinalizerAsync<ProbeFinalizer>(entity);

    var map = entity.Spec.Type.GetProbe(entity.Spec.Parameters);

    var healthCheck = map.probe.Reconcile(map.parameters);
    var healthCheckResult = healthCheck.CheckHealthAsync(Utils.MockHealthCheckContext(healthCheck));

    var nodeName = Environment.GetEnvironmentVariable("NODE_NAME") ?? "unknown";

    var nodeStatus = entity.Status.NodeStatus.SingleOrDefault(x => x.Name == nodeName);
    if (nodeStatus == null) {
      nodeStatus = new NodeStatus() {
        Name = nodeName
      };
      var listNodeStatus = entity.Status.NodeStatus.ToList();
      listNodeStatus.Add(nodeStatus);
      entity.Status.NodeStatus = listNodeStatus.ToArray();
    }

    nodeStatus.Status = (await healthCheckResult).Status.ToString();

    var allStatus = entity.Status.NodeStatus.Length;
    var healthyStatus = entity.Status.NodeStatus.Count(x => x.Status == "Healthy");

    entity.Status.Status = $"{healthyStatus}/{allStatus}";


    _logger.LogInformation("status {}", entity.Status.Status);

    await _client.UpdateStatus(entity);

    return ResourceControllerResult.RequeueEvent(TimeSpan.FromSeconds(5));
  }

  public Task StatusModifiedAsync(V1Alpha1ProbeEntity entity) {
    return Task.CompletedTask;
  }
}