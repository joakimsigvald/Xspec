using System.Numerics;

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
/// Produces the source values of a type relay: from the start (default one),
/// each subsequent value is computed by the step function (default increment by one).
/// Values are guaranteed unique; producing a duplicate throws ValuesExhausted.
/// </summary>
internal class NumericSequence<TSource> : ISequence where TSource : INumber<TSource>
{
    private readonly HashSet<TSource> _produced = [];
    private TSource _start = default!;
    private Func<TSource, int, TSource>? _step;
    private string? _startExpr;
    private string? _stepExpr;
    private bool _started;
    private TSource _current = default!;

    private TSource Start => _startExpr is null ? TSource.One : _start;
    private Func<TSource, int, TSource> Step => _step ?? Increment;

    internal void SetStart(TSource start, string startExpr)
    {
        if (_startExpr is not null)
            throw new SetupFailed("StartingAt can only be applied once per From");
        _start = start;
        _startExpr = startExpr;
    }

    internal void SetStep(Func<TSource, int, TSource> step, string stepExpr)
    {
        if (_stepExpr is not null)
            throw new SetupFailed("Spaced can only be applied once per From");
        _step = step;
        _stepExpr = stepExpr;
    }

    public object? Next()
    {
        _current = _started ? Step(_current, _produced.Count) : Start;
        _started = true;
        return _produced.Add(_current)
            ? _current
            : throw new ValuesExhausted(typeof(TSource));
    }

    public string Describe()
        => (_startExpr is null ? string.Empty : $" starting at {_startExpr}")
        + (_stepExpr is null ? string.Empty : $" spaced {_stepExpr}");

    private static TSource Increment(TSource current, int _) => current + TSource.One;
}
