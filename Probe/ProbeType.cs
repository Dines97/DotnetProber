using Moq;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Prober.Probe;

public enum ProbeType {
  RabbitMq,
  Postgresql
}

public class ProbeTypeMap {
  public ProbeType ProbeType { get; init; }
  public Type Probe { get; init; }
  public Type ProbeParameters { get; init; }
}

public static class ProbeTypeExtensions {
  private static readonly IDeserializer _deserializer =
    new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();


  private static readonly IList<ProbeTypeMap> Probes = new List<ProbeTypeMap>() {
    new ProbeTypeMap() {
      ProbeType = ProbeType.RabbitMq,
      Probe = typeof(RabbitMqProbe),
      ProbeParameters = typeof(RabbitMqParameters)
    },
    new ProbeTypeMap() {
      ProbeType = ProbeType.Postgresql,
      Probe = typeof(PostgresqlProbe),
      ProbeParameters = typeof(PostgresqlParameters)
    }
  };

  public static (IProbe<IProbeParameters> probe, IProbeParameters parameters) GetProbe(this ProbeType probeType,
    string parameters) {
    var probeMap = Probes.Single(x => x.ProbeType == probeType);
    var probe = Activator.CreateInstance(probeMap.Probe) as IProbe<IProbeParameters>;
    var probeParameters = _deserializer.Deserialize(parameters, probeMap.ProbeParameters) as IProbeParameters;

    return (probe, probeParameters);
  }
}