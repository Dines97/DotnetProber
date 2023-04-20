using Prober.Utils;
using UnitsNet;
using UnitsNet.Units;

namespace Prober.ProbeParameters;

public class UriRequestSizeParameters {
  public string Uri { get; set; }

  public string HttpMethod { get; set; }
  public int HttpCode { get; set; }

  public InformationAdapter RequestSize { get; set; } = new() {
    Value = new Information(0, InformationUnit.Bit)
  };

  public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}