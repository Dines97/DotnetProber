using HealthChecks.Uris;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prober.ProbeParameters;
using YamlDotNet.Serialization;

namespace Prober.Probe;

public class UriRequestSizeProbe : IProbe {
  private readonly IDeserializer _deserializer;
  private UriRequestSizeParameters _parameters = null!;

  public UriRequestSizeProbe(IDeserializer deserializer) {
    _deserializer = deserializer;
  }

  public bool Check(ProbeType probeType) {
    return probeType == ProbeType.UriRequestSize;
  }

  public void SetParameters(string parameters) {
    _parameters = _deserializer.Deserialize<UriRequestSizeParameters>(parameters);
  }

  public IHealthCheck Reconcile() {
    var uriHealthCheckOptions = new UriHealthCheckOptions()
      .AddUri(new Uri(_parameters.Uri),
        s => {
          s.UseHttpMethod(new HttpMethod(_parameters.HttpMethod))
            .ExpectHttpCode(_parameters.HttpCode);

          foreach (var header in _parameters.Headers) s.AddCustomHeader(header.Key, header.Value);

          var random = new Random();
          var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

          var randomString = new string(Enumerable.Repeat(chars, (int)_parameters.RequestSize.Value.Bytes)
            .Select(c => c[random.Next(c.Length)]).ToArray());

          s.AddCustomHeader("Cookie", $"{randomString}");
        });

    return new UriHealthCheck(uriHealthCheckOptions, () => new HttpClient());
  }
}