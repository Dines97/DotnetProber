using HealthChecks.Network.Core;

namespace Prober.ProbeParameters;

public class SmtpParameters {
  public string Host { get; set; }

  public int Port { get; set; }

  public bool AllowInvalidRemoteCertificates { get; set; }

  public SmtpConnectionType ConnectionType { get; set; } = SmtpConnectionType.AUTO;

  public string Username { get; set; }

  public string Password { get; set; }
}