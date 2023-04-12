using Prober.Probe;

namespace Prober.Dto;

public class ProbeDto {
  public string Name { get; set; }

  public ProbeType Type { get; set; }

  public string Status { get; set; }


  public IList<NodeStatusDto> NodeStatus { get; set; } = new List<NodeStatusDto>();
}

public class NodeStatusDto {
  public string Name { get; set; }
  public string Status { get; set; } = "Unknown";

  public string Timestamp { get; set; }
}