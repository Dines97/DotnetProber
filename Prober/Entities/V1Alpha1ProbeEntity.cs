using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;
using Prober.Probe;

namespace Prober.Entities;

[KubernetesEntity(Group = "d-teknoloji.com.tr", ApiVersion = "v1alpha1", Kind = "Probe", PluralName = "probes")]
public class V1Alpha1ProbeEntity : CustomKubernetesEntity<V1Alpha1ProbeEntitySpec, V1Alpha1ProbeEntityStatus> { }

public class V1Alpha1ProbeEntitySpec {
  public string Period { get; set; } = "30s";

  public string Timeout { get; set; } = "5s";

  [AdditionalPrinterColumn(Name = "Probe type", Priority = 0)]
  public ProbeType Type { get; set; }

  public Source[] Source { get; set; }
  public Destination Destination { get; set; }
}

public class Source {
  public string Name { get; set; }
  public ObjectType Type { get; set; }
  public Key[] Keys { get; set; }
}

public enum ObjectType {
  ConfigMap,
  Secret
}

public class Key {
  public string Name { get; set; }
  public KeyType Type { get; set; }
  public Dictionary<string, string> Variables { get; set; }
}

public enum KeyType {
  Json,
  Xml
}

public class Destination {
  public ProbeParametersType Type { get; set; }
  public Parameters Parameters { get; set; }
}

public enum ProbeParametersType {
  ConnectionString
}

public class Parameters {
  public string ConnectionString { get; set; }
}

public class V1Alpha1ProbeEntityStatus {
  [AdditionalPrinterColumn(Name = "Probe status", Priority = 0)]
  public string Status { get; set; } = "0/0";
}