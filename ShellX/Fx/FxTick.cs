using ProjectX.Core;

namespace ShellX.Fx;

public record FxTick(DateTimeOffset Timestamp, SpotPrice Price);

/// <summary>The most recent ticks, capped so a long stream does not grow without bound.</summary>
public class FxTickBuffer(int capacity = 200)
{
    private readonly List<FxTick> _ticks = new();

    public int Count => _ticks.Count;

    /// <summary>For the tick list on screen.</summary>
    public IEnumerable<FxTick> NewestFirst => Enumerable.Reverse(_ticks);

    /// <summary>For the chart.</summary>
    public IEnumerable<FxTick> OldestFirst => _ticks;

    public void Add(FxTick tick)
    {
        _ticks.Add(tick);
        if (_ticks.Count > capacity)
            _ticks.RemoveAt(0);
    }

    public void Clear() => _ticks.Clear();
}
