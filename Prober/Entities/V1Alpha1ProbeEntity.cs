using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;
using Prober.Probe;

namespace Prober.Entities;

[KubernetesEntity(Group = "d-teknoloji.com.tr", ApiVersion = "v1alpha1", Kind = "Probe", PluralName = "probes")]
public class V1Alpha1ProbeEntity : CustomKubernetesEntity<V1Alpha1ProbeEntitySpec, V1Alpha1ProbeEntityStatus> { }

public class V1Alpha1ProbeEntitySpec {
  public string Description { get; set; } = string.Empty;

  public string Period { get; set; } = "30s";

  public string Timeout { get; set; } = "5s";

  [AdditionalPrinterColumn(Name = "Probe type", Priority = 0)]
  public ProbeType Type { get; set; }

  public string Parameters { get; set; }
}

public class V1Alpha1ProbeEntityStatus {
  [AdditionalPrinterColumn(Name = "Probe status", Priority = 0)]
  public string Status { get; set; } = "0/0";

  public NodeStatus[] NodeStatus { get; set; } = Array.Empty<NodeStatus>();
}

public class NodeStatus {
  public string Name { get; set; }
  public string Status { get; set; } = "Unknown";

  public string Exception { get; set; } = string.Empty;

  public string Timestamp { get; set; }
}