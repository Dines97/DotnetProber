using System.Text.RegularExpressions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Prober.Utils;

public static class Utils {
  private static readonly IDictionary<string, Func<int, TimeSpan>?> TimeQuantityMap =
    new Dictionary<string, Func<int, TimeSpan>?> {
      { "s", x => TimeSpan.FromSeconds(x) },
      { "m", x => TimeSpan.FromMinutes(x) }
    };

  private static readonly Regex quantityRegex = new(@"^(\d+)(\D*)$", RegexOptions.Compiled);

  public static bool TryQuantityRegexSplit(string input, out string[]? output) {
    output = null;

    var match = quantityRegex.Match(input);
    if (match.Groups.Count != 3) return false;

    output = new[] { match.Groups[1].Value, match.Groups[2].Value };
    return true;
  }

  public static HealthCheckContext MockHealthCheckContext(IHealthCheck instance) {
    var healthCheckContext = new HealthCheckContext {
      Registration =
        new HealthCheckRegistration(string.Empty,
          instance,
          HealthStatus.Degraded,
          Array.Empty<string>())
    };

    return healthCheckContext;
  }


  public static TimeSpan ParseTimeSpan(string value) {
    var match = quantityRegex.Match(value);

    TimeQuantityMap.TryGetValue(match.Groups[2].Value, out var timeSpanFunction);

    return timeSpanFunction?.Invoke(int.Parse(match.Groups[1].Value)) ?? TimeSpan.FromSeconds(30);
  }

  public static bool CompareDictionary<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
    where TValue : IEquatable<TValue> {
    return left.Keys.All(key => right.ContainsKey(key) && right[key].Equals(left[key]));
  }

  // public static TimeSpan ParseSize(string value) {
  //   var match = quantityRegex.Match(value);
  //
  //   SizeQuantityMap.TryGetValue(match.Groups[2].Value, out var size);
  //
  //   return timeSpanFunction?.Invoke(int.Parse(match.Groups[1].Value)) ?? TimeSpan.FromSeconds(30);
  // }
}