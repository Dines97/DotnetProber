using System.Globalization;
using Prober.Utils;
using Shouldly;
using UnitsNet;
using UnitsNet.Units;
using Xunit.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tests;

internal class Test {
  public InformationAdapter Value { get; set; }
}

public class Experiments {
  private readonly ITestOutputHelper _testOutputHelper;

  public Experiments(ITestOutputHelper testOutputHelper) {
    _testOutputHelper = testOutputHelper;
  }

  [Fact]
  public void Test1() {
    var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
    var serialiazer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

    var s = @"
value: 1Ki
    ";

    var test = deserializer.Deserialize<Test>(s);

    var aaa = serialiazer.Serialize(test);
    _testOutputHelper.WriteLine(aaa);

    _testOutputHelper.WriteLine(test.Value.Value.Bytes.ToString(CultureInfo.InvariantCulture));
  }

  [Fact]
  public void UnitsNet() {
    new Information(1, InformationUnit.Kilobyte).Bytes.ShouldBe(1000);
    new Information(1, InformationUnit.Kibibyte).Bytes.ShouldBe(1024);
  }
}