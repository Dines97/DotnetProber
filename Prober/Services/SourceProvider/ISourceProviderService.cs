using Prober.Entities;

namespace Prober.Services.SourceProvider;

public interface ISourceProviderService {
  public Task<Dictionary<string, string>> GetVariablesAsync(IEnumerable<Source> sources, string fallBackNamespace);
}