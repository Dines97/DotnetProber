using AutoMapper;
using KubeOps.KubernetesClient;
using Microsoft.AspNetCore.Mvc;
using Prober.Dto;
using Prober.Entities;
using Prober.Probe;

namespace Prober.ApiControllers;

[ApiController]
[Route("[controller]/[action]")]
public class HealthCheckController {
  private readonly IKubernetesClient _client;
  private readonly IMapper _mapper;

  public HealthCheckController(IKubernetesClient client, IMapper mapper) {
    _client = client;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IEnumerable<ProbeDto>> Healthcheck() {
    var probes = await _client.List<V1Alpha1ProbeEntity>();

    return _mapper.Map<IEnumerable<V1Alpha1ProbeEntity>, IEnumerable<ProbeDto>>(probes);
  }

  [HttpGet]
  public async Task<IEnumerable<ProbeDto>> HealthCheckByLabels([FromQuery] Dictionary<string, string> labels) {
    var allProbes = await _client.List<V1Alpha1ProbeEntity>();

    var suitableProbes = allProbes.Where(p => Utils.Utils.CompareDictionary(labels, p.Metadata.Labels)).ToList();

    return _mapper.Map<IEnumerable<V1Alpha1ProbeEntity>, IEnumerable<ProbeDto>>(suitableProbes);
  }


  [HttpGet]
  public async Task<IEnumerable<ProbeDto>> HealthChecksByType([FromQuery] ProbeType probeType) {
    var allProbes = await _client.List<V1Alpha1ProbeEntity>();

    var suitableProbes = allProbes.Where(p => p.Spec.Type == probeType).ToList();

    return _mapper.Map<IEnumerable<V1Alpha1ProbeEntity>, IEnumerable<ProbeDto>>(suitableProbes);
  }
}