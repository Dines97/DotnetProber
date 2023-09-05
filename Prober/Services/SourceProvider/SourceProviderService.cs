using k8s;
using k8s.Models;
using KubeOps.KubernetesClient;
using Prober.Entities;

namespace Prober.Services.SourceProvider;

public class SourceProviderService : ISourceProviderService {
  private readonly IKubernetesClient _client;

  public SourceProviderService(IKubernetesClient client) {
    _client = client;
  }

  public async Task<Dictionary<string, string>>
    GetVariablesAsync(IEnumerable<Source> sources, string fallBackNamespace) {
    var variables = new Dictionary<string, string>();

    foreach (var source in sources) {
      var parts = source.Name.Split('/');
      string @namespace, name;
      if (parts.Length >= 2) {
        @namespace = parts[0];
        name = parts[1];
      }
      else {
        @namespace = fallBackNamespace;
        name = source.Name;
      }

      var data = await GetDataFromConfigMapOrSecret(name, @namespace, source.Type);

      GetVariablesFromDictionary(source.Keys, data, variables);
    }


    return variables;
  }

  private async Task<IDictionary<string, string>> GetDataFromConfigMapOrSecret(string name, string @namespace,
    ObjectType objectType) {
    return objectType switch {
      ObjectType.ConfigMap => await GetDataFromConfigMapOrSecret<V1ConfigMap>(name, @namespace),
      ObjectType.Secret => await GetDataFromConfigMapOrSecret<V1Secret>(name, @namespace),
      _ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, null)
    };
  }

  private async Task<IDictionary<string, string>> GetDataFromConfigMapOrSecret<T>(string name, string @namespace)
    where T : class, IKubernetesObject<V1ObjectMeta>, IValidate {
    var result = await _client.Get<T>(name, @namespace);
    return result switch {
      V1ConfigMap configMap => configMap.Data,
      V1Secret secret => secret.StringData,
      _ => throw new InvalidOperationException(
        $"""{typeof(T).FullName} with name "{name}" doesn't exist in "{@namespace}" namespace""")
    };
  }

  private void GetVariablesFromDictionary(IEnumerable<Key> keys, IDictionary<string, string> sourceDictionary,
    IDictionary<string, string> targetDictionary) { }
}