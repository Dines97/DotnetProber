using System.Security.Cryptography;
using System.Text;
using k8s;
using k8s.Models;
using KubeOps.KubernetesClient;
using KubeOps.Operator;
using KubeOps.Operator.Entities.Extensions;
using KubeOps.Operator.Events;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.Entities;
using Prober.Probe;
using Prober.Services.SourceProvider;
using SimpleBase;

namespace Prober.ProbeManager;

public class ProbeManager : IProbeManager {
  private readonly IKubernetesClient _client;

  private readonly IEventManager _eventManager;
  private readonly ILogger<ProbeManager> _logger;
  private readonly IEnumerable<IProbe> _probes;
  private readonly OperatorSettings _settings;
  private readonly ISourceProviderService _sourceProviderService;

  public ProbeManager(IEnumerable<IProbe> probes, ILogger<ProbeManager> logger, IEventManager eventManager,
    IKubernetesClient client, OperatorSettings settings, ISourceProviderService sourceProviderService) {
    _probes = probes;
    _logger = logger;
    _eventManager = eventManager;
    _client = client;
    _settings = settings;
    _sourceProviderService = sourceProviderService;
  }

  public async Task ReconciledAsync(V1Alpha1ProbeEntity entity) {
    var probe = _probes.SingleOrDefault(x => x.Check(entity.Spec.Type));
    if (probe == null) {
      _logger.LogInformation("Doesn't found probe that satisfies {}", entity.Spec.Type);
      return;
    }

    // probe.SetParameters(entity.Spec.Parameters);
    await _sourceProviderService.GetVariablesAsync(entity.Spec.Source, "default");
    var nodeName = Environment.GetEnvironmentVariable("NODE_NAME") ?? "unknown";

    var healthCheck = probe.Reconcile();
    var healthCheckResult = await healthCheck.CheckHealthAsync(Utils.Utils.MockHealthCheckContext(healthCheck));

    // await _eventManager.PublishAsync(entity,
    //   "Reconcile",
    //   $"Node: {nodeName} | Status: {(healthCheckResult).Status.ToString()} | Exception: {healthCheckResult.Exception?.Message ?? string.Empty}",
    //   healthCheckResult.Status == HealthStatus.Healthy ? EventType.Normal : EventType.Warning);

    await PublishProbeEventAsync(entity, "Reconcile",
      $"Node: {nodeName} | Status: {healthCheckResult.Status.ToString()} | Exception: {healthCheckResult.Exception?.Message ?? string.Empty}",
      nodeName, healthCheckResult.Status.ToString(),
      healthCheckResult.Status == HealthStatus.Healthy ? EventType.Normal : EventType.Warning
    );

    // var allStatus = entity.Status.NodeStatus.Length;
    // var healthyStatus = entity.Status.NodeStatus.Count(x => x.Status == "Healthy");
    //
    // entity.Status.Status = $"{healthyStatus}/{allStatus}";

    _logger.LogInformation("status {}", entity.Status.Status);
  }

  public Task StatusModifiedAsync(V1Alpha1ProbeEntity entity) {
    throw new NotImplementedException();
  }


  private async Task PublishProbeEventAsync(IKubernetesObject<V1ObjectMeta> resource,
    string reason,
    string message,
    string node,
    string status,
    EventType type = EventType.Normal) {
    var resourceNamespace = resource.Namespace() ?? "default";

    var eventName =
      Base32.Rfc4648.Encode(
        SHA512.HashData(
          Encoding.UTF8.GetBytes($"{resource.Name()}.{resourceNamespace}.{reason}.{message}.{node}.{status}.{type}")));

    var @event = await _client.Get<Corev1Event>(eventName, resourceNamespace) ??
                 new Corev1Event {
                   Kind = Corev1Event.KubeKind,
                   ApiVersion = $"{Corev1Event.KubeGroup}/{Corev1Event.KubeApiVersion}",
                   Metadata = new V1ObjectMeta {
                     Name = eventName,
                     NamespaceProperty = resourceNamespace,
                     Annotations = new Dictionary<string, string> {
                       { "nameHash", "sha512" }, { "nameEncoding", "Base32 / RFC 4648" },
                       { "d-teknoloji.com.tr.prober/node", node }, { "d-teknoloji.com.tr.prober/status", status }
                     }
                   },
                   Type = type.ToString(),
                   Reason = reason,
                   Message = message,
                   ReportingComponent = _settings.Name,
                   ReportingInstance = Environment.MachineName,
                   Source = new V1EventSource { Component = _settings.Name },
                   InvolvedObject = resource.MakeObjectReference(),
                   EventTime = DateTime.Now,
                   FirstTimestamp = DateTime.UtcNow,
                   LastTimestamp = DateTime.UtcNow,
                   Action = "Reconcile",
                   Count = 0
                 };
    @event.Count++;
    @event.LastTimestamp = DateTime.UtcNow;

    await _client.Save(@event);
  }
}