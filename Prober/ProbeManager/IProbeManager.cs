using Prober.Entities;

namespace Prober.ProbeManager;

public interface IProbeManager {
  Task ReconciledAsync(V1Alpha1ProbeEntity entity);

  Task StatusModifiedAsync(V1Alpha1ProbeEntity entity);
}