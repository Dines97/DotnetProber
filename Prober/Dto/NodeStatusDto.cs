namespace Prober.Dto;

public class NodeStatusDto {
  public string Name { get; set; }
  public string Status { get; set; } = "Unknown";

  public string Timestamp { get; set; }
}