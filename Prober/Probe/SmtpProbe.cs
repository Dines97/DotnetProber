using HealthChecks.Network;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;

namespace Prober.Probe;

public class SmtpProbe : IProbe {
  private readonly IDeserializer _deserializer;
  private SmtpParameters _parameters = null!;

  public SmtpProbe(IDeserializer deserializer) {
    _deserializer = deserializer;
  }


  public bool Check(ProbeType probeType) {
    return probeType == ProbeType.Smtp;
  }

  public void SetParameters(string parameters) {
    _parameters = _deserializer.Deserialize<SmtpParameters>(parameters);
  }

  public IHealthCheck Reconcile() {
    var options = new SmtpHealthCheckOptions {
      Host = _parameters.Host,
      Port = _parameters.Port,
      AllowInvalidRemoteCertificates = _parameters.AllowInvalidRemoteCertificates,
      ConnectionType = _parameters.ConnectionType
    };
    options.LoginWith(_parameters.Username, _parameters.Password);

    return new SmtpHealthCheck(options);
  }
}