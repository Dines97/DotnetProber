using AutoMapper;
using KubeOps.KubernetesClient;
using Microsoft.AspNetCore.Mvc;
using Prober.Dto;
using Prober.Entities;

namespace Prober.ApiController;

[ApiController]
[Route("[controller]")]
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
  //
  // public static ProbeDto ToProbeDto(V1Alpha1ProbeEntity probe) {
  //   var probeDto = new ProbeDto() {
  //     Name = probe.Metadata.Name,
  //     Type = probe.Spec.Type,
  //     Status = probe.Status.Status
  //   };
  //
  //   foreach (var nodeStatus in probe.Status.NodeStatus) {
  //     probeDto.NodeStatus.Add(new NodeStatusDto() {
  //       Name = nodeStatus.Name,
  //       Status = nodeStatus.Status,
  //       Timestamp = nodeStatus.Timestamp
  //     });
  //   }
  //
  //   return probeDto;
  // }
}