using System.Globalization;
using Prober.Entities;
using Prober.Probe;

namespace Prober.ProbeManager;

public class ProbeManager : IProbeManager {
  private readonly ILogger<ProbeManager> _logger;
  private readonly IEnumerable<IProbe> _probes;


  public ProbeManager(IEnumerable<IProbe> probes, ILogger<ProbeManager> logger) {
    _probes = probes;
    _logger = logger;
  }

  public async Task ReconciledAsync(V1Alpha1ProbeEntity entity) {
    var probe = _probes.SingleOrDefault(x => x.Check(entity.Spec.Type));
    if (probe == null) {
      _logger.LogInformation("Doesn't found probe that satisfies {}", entity.Spec.Type);
      return;
    }

    probe.SetParameters(entity.Spec.Parameters);

    var healthCheck = probe.Reconcile();
    var healthCheckResult = healthCheck.CheckHealthAsync(Utils.Utils.MockHealthCheckContext(healthCheck));

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
    nodeStatus.Exception = healthCheckResult.Exception?.Message ?? string.Empty;
    nodeStatus.Timestamp = DateTime.Now.ToString("O", CultureInfo.InvariantCulture);

    var allStatus = entity.Status.NodeStatus.Length;
    var healthyStatus = entity.Status.NodeStatus.Count(x => x.Status == "Healthy");

    entity.Status.Status = $"{healthyStatus}/{allStatus}";

    _logger.LogInformation("status {}", entity.Status.Status);
  }

  public Task StatusModifiedAsync(V1Alpha1ProbeEntity entity) {
    throw new NotImplementedException();
  }
}