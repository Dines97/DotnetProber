using HealthChecks.Network;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;

namespace Prober.Probe;

public class DnsResolveProbe : IProbe {
  private readonly IDeserializer _deserializer;
  private DnsResolveParameters _parameters = null!;

  public DnsResolveProbe(IDeserializer deserializer) {
    _deserializer = deserializer;
  }


  public bool Check(ProbeType probeType) {
    return probeType == ProbeType.DnsResolve;
  }

  public void SetParameters(string parameters) {
    _parameters = _deserializer.Deserialize<DnsResolveParameters>(parameters);
  }

  public IHealthCheck Reconcile() {
    return new DnsResolveHealthCheck(new DnsResolveOptions().ResolveHost(_parameters.Host).To(_parameters.Resolutions));
  }
}