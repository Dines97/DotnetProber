using UnitsNet;
using UnitsNet.Units;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace Prober.Utils;

public class InformationAdapter : IYamlConvertible {
  private readonly Dictionary<string, InformationUnit> InformationMap = new() {
    { "", InformationUnit.Byte },
    { "k", InformationUnit.Kilobyte },
    { "M", InformationUnit.Megabyte },
    { "G", InformationUnit.Gigabyte },
    { "T", InformationUnit.Terabyte },
    { "P", InformationUnit.Petabyte },
    { "E", InformationUnit.Exabyte },
    { "Ki", InformationUnit.Kibibyte },
    { "Mi", InformationUnit.Mebibyte },
    { "Gi", InformationUnit.Gibibyte },
    { "Ti", InformationUnit.Tebibyte },
    { "Pi", InformationUnit.Pebibyte },
    { "Ei", InformationUnit.Exbibyte }
  };

  public Information Value { get; set; }

  public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) {
    var scalar = parser.Consume<Scalar>();
    var success = Utils.TryQuantityRegexSplit(scalar.Value, out var parts);
    if (!success) throw new YamlException("Can't convert unit");


    Value = Information.From(int.Parse(parts![0]), InformationMap[parts[1]]);
  }

  public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) {
    emitter.Emit(new Scalar($"{Value.Value}{InformationMap.First(x => x.Value == Value.Unit).Key}"));
  }
}