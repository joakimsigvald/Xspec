namespace Xspec.Internal.TestData.Generation;

/// <summary>
/// A source value sequence configured with StartingAt and Spaced.
/// </summary>
internal interface ISequence
{
    object? Next();
    string Describe();
}

/// <summary>
/// Produces the source values of a type relay: from the start,
/// each subsequent value is computed by the step function.
/// The concrete sequence type provides the default start and step for its value type.
/// Values are guaranteed unique; producing a duplicate, or stepping outside the value type's range,
/// throws ValuesExhausted.
/// </summary>
internal abstract class Sequence<TValue> : ISequence
{
    private readonly HashSet<TValue> _produced = [];
    private TValue _start = default!;
    private Func<TValue, int, TValue>? _step;
    private string? _startExpr;
    private string? _stepExpr;
    private bool _started;
    private TValue _current = default!;

    private TValue Start => _startExpr is null ? DefaultStart : _start;
    private Func<TValue, int, TValue> Step => _step ?? DefaultStep;

    protected abstract TValue DefaultStart { get; }
    protected abstract TValue DefaultStep(TValue current, int position);

    internal void SetStart(TValue start, string startExpr)
    {
        if (_startExpr is not null)
            throw new SetupFailed("StartingAt can only be applied once per From");
        _start = start;
        _startExpr = startExpr;
    }

    internal void SetStep(Func<TValue, int, TValue> step, string stepExpr)
    {
        if (_stepExpr is not null)
            throw new SetupFailed("Spaced can only be applied once per From");
        _step = step;
        _stepExpr = stepExpr;
    }

    public object? Next()
    {
        _current = _started ? Advance() : Start;
        _started = true;
        return _produced.Add(_current)
            ? _current
            : throw new ValuesExhausted(typeof(TValue));
    }

    private TValue Advance()
    {
        try
        {
            return Step(_current, _produced.Count);
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or OverflowException)
        {
            throw new ValuesExhausted(typeof(TValue));
        }
    }

    public string Describe()
        => (_startExpr is null ? string.Empty : $" starting at {_startExpr}")
        + (_stepExpr is null ? string.Empty : $" spaced {_stepExpr}");
}
