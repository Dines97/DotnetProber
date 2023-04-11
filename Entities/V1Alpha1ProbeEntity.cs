using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.Probe;

namespace Prober.Entities;

[KubernetesEntity(Group = "dteknoloji.com.tr", ApiVersion = "v1alpha1", Kind = "Probe", PluralName = "probes")]
public class V1Alpha1ProbeEntity : CustomKubernetesEntity<V1Alpha1ProbeEntitySpec, V1Alpha1ProbeEntityStatus> { }

public class V1Alpha1ProbeEntitySpec {
  public string Description { get; set; } = string.Empty;

  public int Period { get; set; } = 1;

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

  public string Timestamp { get; set; }
}