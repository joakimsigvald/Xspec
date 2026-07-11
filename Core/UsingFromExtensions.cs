using System.Numerics;
using System.Runtime.CompilerServices;
using Xspec.Continuations;
using Xspec.Internal.Pipelines;
using Xspec.Internal.TestData.Generation;

namespace Xspec;

/// <summary>
/// Sequence constraints for From&lt;TSource&gt;() with a numeric or DateTime source type,
/// controlling which values the generator produces for the registered type relay.
/// Generated values are guaranteed to be unique: fixed numeric spacings wrap around at the type's
/// boundaries, and generation throws ValuesExhausted as soon as the sequence would repeat a value.
/// </summary>
public static class UsingFromExtensions
{
    /// <summary>
    /// Instructs the generator to produce source values starting at the given value.
    /// Unless a spacing is provided with Spaced, consecutive values are one apart.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="start">The first value to generate.</param>
    /// <param name="startExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TSource> StartingAt<TSUT, TResult, TSource>(
        this IUsingFromContinuation<TSUT, TResult, TSource> pipeline,
        TSource start,
        [CallerArgumentExpression(nameof(start))] string? startExpr = null)
        where TSource : INumber<TSource>
    {
        Sequence<NumericSequence<TSource>>(pipeline).SetStart(start, startExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values with the given spacing between consecutive values.
    /// A negative spacing yields a descending sequence. Unless a start is provided with StartingAt, the sequence starts at one.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">The difference between consecutive values. Cannot be zero.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TSource> Spaced<TSUT, TResult, TSource>(
        this IUsingFromContinuation<TSUT, TResult, TSource> pipeline,
        TSource spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
        where TSource : INumber<TSource>
    {
        if (TSource.IsZero(spacing))
            throw new SetupFailed("Spacing cannot be zero");
        Sequence<NumericSequence<TSource>>(pipeline).SetStep((current, _) => current + spacing, spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values by applying the given step function to the previous value.
    /// Unless a start is provided with StartingAt, the sequence starts at one.
    /// The step function may produce any sequence, as long as it never repeats a value.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">A function computing the next value from the previous one.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TSource> Spaced<TSUT, TResult, TSource>(
        this IUsingFromContinuation<TSUT, TResult, TSource> pipeline,
        Func<TSource, TSource> spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
        where TSource : INumber<TSource>
    {
        if (spacing is null)
            throw new SetupFailed("Spacing cannot be null");
        Sequence<NumericSequence<TSource>>(pipeline).SetStep((current, _) => spacing(current), spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values by applying the given step function
    /// to the previous value and the position of the value being generated (the start value has position 0),
    /// e.g. StartingAt(0).Spaced((_, i) => i * i) yields 0, 1, 4, 9, ...
    /// Unless a start is provided with StartingAt, the sequence starts at one.
    /// The step function may produce any sequence, as long as it never repeats a value.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">A function computing the next value from the previous value and the position of the next value.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TSource> Spaced<TSUT, TResult, TSource>(
        this IUsingFromContinuation<TSUT, TResult, TSource> pipeline,
        Func<TSource, int, TSource> spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
        where TSource : INumber<TSource>
    {
        if (spacing is null)
            throw new SetupFailed("Spacing cannot be null");
        Sequence<NumericSequence<TSource>>(pipeline).SetStep(spacing, spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values starting at the given value.
    /// Unless a spacing is provided with Spaced, consecutive values are one day apart.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="start">The first value to generate.</param>
    /// <param name="startExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, DateTime> StartingAt<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, DateTime> pipeline,
        DateTime start,
        [CallerArgumentExpression(nameof(start))] string? startExpr = null)
    {
        Sequence<DateTimeSequence>(pipeline).SetStart(start, startExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values with the given spacing between consecutive values.
    /// A negative spacing yields a descending sequence.
    /// Unless a start is provided with StartingAt, the sequence starts at the data generator's epoch.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">The difference between consecutive values. Cannot be zero.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, DateTime> Spaced<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, DateTime> pipeline,
        TimeSpan spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
    {
        if (spacing == TimeSpan.Zero)
            throw new SetupFailed("Spacing cannot be zero");
        Sequence<DateTimeSequence>(pipeline).SetStep((current, _) => current + spacing, spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values by applying the given step function to the previous value.
    /// Unless a start is provided with StartingAt, the sequence starts at the data generator's epoch.
    /// The step function may produce any sequence, as long as it never repeats a value.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">A function computing the next value from the previous one.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, DateTime> Spaced<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, DateTime> pipeline,
        Func<DateTime, DateTime> spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
    {
        if (spacing is null)
            throw new SetupFailed("Spacing cannot be null");
        Sequence<DateTimeSequence>(pipeline).SetStep((current, _) => spacing(current), spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values by applying the given step function
    /// to the previous value and the position of the value being generated (the start value has position 0).
    /// Unless a start is provided with StartingAt, the sequence starts at the data generator's epoch.
    /// The step function may produce any sequence, as long as it never repeats a value.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">A function computing the next value from the previous value and the position of the next value.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, DateTime> Spaced<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, DateTime> pipeline,
        Func<DateTime, int, DateTime> spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
    {
        if (spacing is null)
            throw new SetupFailed("Spacing cannot be null");
        Sequence<DateTimeSequence>(pipeline).SetStep(spacing, spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values starting at the given value.
    /// Unless a spacing is provided with Spaced, consecutive values are one day apart.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="start">The first value to generate.</param>
    /// <param name="startExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TimeSpan> StartingAt<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, TimeSpan> pipeline,
        TimeSpan start,
        [CallerArgumentExpression(nameof(start))] string? startExpr = null)
    {
        Sequence<TimeSpanSequence>(pipeline).SetStart(start, startExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values with the given spacing between consecutive values.
    /// A negative spacing yields a descending sequence.
    /// Unless a start is provided with StartingAt, the sequence starts at one day.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">The difference between consecutive values. Cannot be zero.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TimeSpan> Spaced<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, TimeSpan> pipeline,
        TimeSpan spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
    {
        if (spacing == TimeSpan.Zero)
            throw new SetupFailed("Spacing cannot be zero");
        Sequence<TimeSpanSequence>(pipeline).SetStep((current, _) => current + spacing, spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values by applying the given step function to the previous value.
    /// Unless a start is provided with StartingAt, the sequence starts at one day.
    /// The step function may produce any sequence, as long as it never repeats a value.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">A function computing the next value from the previous one.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TimeSpan> Spaced<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, TimeSpan> pipeline,
        Func<TimeSpan, TimeSpan> spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
    {
        if (spacing is null)
            throw new SetupFailed("Spacing cannot be null");
        Sequence<TimeSpanSequence>(pipeline).SetStep((current, _) => spacing(current), spacingExpr!);
        return pipeline;
    }

    /// <summary>
    /// Instructs the generator to produce source values by applying the given step function
    /// to the previous value and the position of the value being generated (the start value has position 0).
    /// Unless a start is provided with StartingAt, the sequence starts at one day.
    /// The step function may produce any sequence, as long as it never repeats a value.
    /// </summary>
    /// <param name="pipeline">The From-continuation whose source values to constrain.</param>
    /// <param name="spacing">A function computing the next value from the previous value and the position of the next value.</param>
    /// <param name="spacingExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>The same continuation, for further sequence constraints or arrangement.</returns>
    public static IUsingFromContinuation<TSUT, TResult, TimeSpan> Spaced<TSUT, TResult>(
        this IUsingFromContinuation<TSUT, TResult, TimeSpan> pipeline,
        Func<TimeSpan, int, TimeSpan> spacing,
        [CallerArgumentExpression(nameof(spacing))] string? spacingExpr = null)
    {
        if (spacing is null)
            throw new SetupFailed("Spacing cannot be null");
        Sequence<TimeSpanSequence>(pipeline).SetStep(spacing, spacingExpr!);
        return pipeline;
    }

    private static TSequence Sequence<TSequence>(object pipeline)
        where TSequence : class, ISequence, new()
    {
        var receiver = pipeline as ISequenceReceiver
            ?? throw new SetupFailed("Sequence constraints require a continuation created by From");
        var sequence = receiver.Sequence as TSequence ?? new();
        receiver.InstallSequence(sequence);
        return sequence;
    }
}
