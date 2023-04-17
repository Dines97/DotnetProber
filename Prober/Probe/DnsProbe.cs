using System.Text.Json;
using System.Text.Json.Nodes;
using HealthChecks.Network;
using KubeOps.Operator.Webhooks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;

namespace Prober.Probe;

public class DnsProbe : IProbe {
  private readonly IDeserializer _deserializer;
  private DnsParameters _parameters = null!;

  public DnsProbe(IDeserializer deserializer) {
    _deserializer = deserializer;
  }


  public bool Check(ProbeType probeType) {
    return probeType == ProbeType.DnsResolve;
  }

  public void SetParameters(string parameters) {
    throw new NotImplementedException();
  }

  public IHealthCheck Reconcile() {
    return new DnsResolveHealthCheck(new DnsResolveOptions().ResolveHost(_parameters.Host).To(_parameters.Resolutions));
  }
}