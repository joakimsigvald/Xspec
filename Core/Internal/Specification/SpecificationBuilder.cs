using Xspec.Internal.TestData;

namespace Xspec.Internal.Specification;

internal class SpecificationBuilder
{
    private readonly List<Action> _applications = [];
    private int _givenCount;
    private int _recordingSuppressionCount;
    private int _thenCount;
    private string? _currentMockSetup;
    private readonly TextBuilder _textBuilder = new();
    private bool _isChainOfAssertions = false;

    private readonly Lazy<string> _lazySpecification;

    internal SpecificationBuilder() => _lazySpecification = new Lazy<string>(Build);

    internal string Specification => _lazySpecification.Value;

    internal void Add(Action apply)
    {
        if (_recordingSuppressionCount > 0)
            return;
        _applications.Add(apply);
    }

    private string Build()
    {
        foreach (var apply in _applications) apply();
        return _textBuilder.ToString();
    }

    internal void AddMockSetup<TService>(string callExpr)
        => _textBuilder.AddPhraseOrSentence($"{Given} {GetMockName<TService>('.')}{callExpr.ParseCall(true)}");

    internal void AddMockReturnsDefault<TService>(string returnsExpr)
        => _textBuilder.AddPhraseOrSentence($"{Given} {GetMockName<TService>(' ')}returns {returnsExpr.ParseValue()}");

    internal void AddMockReturns(string? returnsExpr)
        => _textBuilder.AddWord($"returns {returnsExpr?.ParseValue()}".Trim());

    internal void AddMockThrowsDefault<TService, TException>()
        => _textBuilder.AddWord($"{Given} {GetMockName<TService>(' ')}throws {NameOf<TException>()}");

    internal void AddMockThrowsDefault<TService>(string expectedExpr)
        => _textBuilder.AddWord($"{Given} {GetMockName<TService>(' ')}throws {expectedExpr.ParseValue()}");

    internal void AddMockThrows<TException>()
        => _textBuilder.AddWord($"throws {NameOf<TException>()}");

    internal void AddMockThrows(string expectedExpr)
        => _textBuilder.AddWord($"throws {expectedExpr.ParseValue()}");

    internal void AddWhen(string actExpr)
        => _textBuilder.AddSentence($"when {actExpr.ParseCall()}");

    internal void AddAfter(string setUpExpr)
        => _textBuilder.AddSentence($"after {setUpExpr.ParseCall()}");

    internal void AddBefore(string tearDownExpr)
        => _textBuilder.AddSentence($"before {tearDownExpr.ParseCall()}");

    internal void AddAssert(string actual, string verb, string? expected)
    {
        if (_isChainOfAssertions)
        {
            actual = actual.Split(".That.").Last().Split(".that.").Last();
            _textBuilder.AddWord(actual.ParseValue());
        }
        else
            _textBuilder.AddSentence(actual);
        _textBuilder.AddWord(verb.AsWords());
        _textBuilder.AddWord(expected.ParseValue());
        _isChainOfAssertions = false;
    }

    internal void AddThen()
    {
        _isChainOfAssertions = true;
        _textBuilder.AddPhraseOrSentence(Then);
    }

    internal void AddThat()
    {
        _isChainOfAssertions = true;
        _textBuilder.AddWord(_that);
    }

    internal void AddGiven(string valueExpr, Scope scope)
    {
        _currentMockSetup = null;
        _textBuilder.AddPhraseOrSentence(string.Join(' ', GetWords()));

        IEnumerable<string> GetWords()
        {
            yield return Given;
            if (scope == Scope.Construction)
                yield return "using";
            yield return valueExpr.ParseValue();
            if (scope == Scope.Default)
                yield return "is default";
        }
    }

    internal void AddGiven<TValue>(string setupExpr, bool isCustomExpression, string? article)
    {
        _currentMockSetup = null;
        _textBuilder.AddPhraseOrSentence(GetGivenExpression<TValue>(setupExpr, isCustomExpression, article));
    }

    private string GetGivenExpression<TValue>(string setupExpr, bool isCustomExpression, string? article)
    {
        if (isCustomExpression)
            return $"{Given} {setupExpr}";

        var articleStr = string.IsNullOrEmpty(article) ? string.Empty : $"{article.AsWords()} ";
        return $"{Given} {articleStr}{ParseSetupExpression<TValue>(setupExpr)}";
    }

    private static string ParseSetupExpression<TValue>(string setupExpr)
    {
        var value = setupExpr.ParseValue();
        var verb = value.Contains('=') && !value.StartsWith("new") ? "has" : "is";
        return $"{NameOf<TValue>()} {verb} {value}";
    }

    internal void AddGivenCount<TModel>(string count)
    {
        _currentMockSetup = null;
        var articleStr = string.IsNullOrEmpty(count) ? string.Empty : $"{count.AsWords()} ";
        _textBuilder.AddPhraseOrSentence($"{Given} {articleStr}{NameOf<TModel>()}");
    }

    internal void AddVerify<TService>(string expressionExpr)
        => _textBuilder.AddWord($"{NameOf<TService>()}.{expressionExpr.ParseCall(true)}");

    internal void AddAssertThrows<TError>(string? binder)
        => _textBuilder.AddWord($"throws {NameOf<TError>()} {binder}".Trim());

    internal void AddAssertThrows(string expectedExpr)
        => _textBuilder.AddWord($"throws {expectedExpr.ParseValue()}");

    internal void AddTap(string expr) => _textBuilder.AddWord($"tap({expr})");

    internal void AddAssert(string assertName) => _textBuilder.AddWord(assertName.AsWords());

    internal void AddUnique(string type)
        => _textBuilder.AddPhraseOrSentence($"{Given} all {type} are unique");

    internal void AddAssertConjunction(string conjunction)
    {
        _isChainOfAssertions = true;
        _textBuilder.AddPhrase(conjunction, 2);
    }

    private string GetMockName<TService>(char binder)
    {
        var nextMockSetup = NameOf<TService>();
        var mockName = nextMockSetup == _currentMockSetup
            ? ""
            : $"{nextMockSetup}{binder}";
        _currentMockSetup = nextMockSetup;
        return mockName;
    }

    private static string NameOf<T>() => typeof(T).Alias();

    internal void SuppressRecording() => _recordingSuppressionCount++;

    internal void InciteRecording() => _recordingSuppressionCount--;

    private string Given => 0 == _givenCount++ ? "Given" : "and";

    private string Then => 0 == _thenCount++ ? "Then" : "and";

    private const string _that = "that";
}