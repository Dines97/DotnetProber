using k8s.Models;
using Microsoft.AspNetCore.Mvc;
using Prober.Entities;
using Prober.ProbeManager;

namespace Prober.ApiControllers;

[ApiController]
[Route("[controller]/[action]")]
public class NodeController {
  private readonly ILogger<NodeController> _logger;
  private readonly IProbeManager _manager;

  public NodeController(IProbeManager manager, ILogger<NodeController> logger) {
    _manager = manager;
    _logger = logger;
  }

  [HttpGet]
  public async Task<V1Alpha1ProbeEntity> Reconcile([FromBody] V1Alpha1ProbeEntity entity) {
    _logger.LogInformation("entity {} called {}", entity.Name(), nameof(Reconcile));

    await _manager.ReconciledAsync(entity);

    return entity;
  }
}