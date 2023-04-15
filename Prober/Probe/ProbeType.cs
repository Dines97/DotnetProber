using Moq;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Prober.Probe;

public enum ProbeType {
  RabbitMq,
  Postgresql,
  DnsProbe
}