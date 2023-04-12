namespace Prober.ProbeParameters;

public class PostgresqlParameters : IProbeParameters {
  public string? ConnectionString { get; set; }
}