using Moq;
using Xspec.Internal.Specification;
using Xspec.Internal.TestData;

namespace Xspec.Internal.Pipelines;

internal abstract class Fixture<TSUT>
{
    private protected readonly Context _context = new();
    private protected readonly SpecFixture<TSUT> _fixture = new();
    private protected readonly Arranger _arranger = new();
    private protected Command? _methodUnderTest;

    public void TearDown()
    {
        if (_fixture.IsSetUp)
            _fixture.Dispose();
    }

    internal void SetDefault<TModel>(
        Action<TModel> setup, string setupExpr) where TModel : class
    {
        SpecificationGenerator.AddGiven<TModel>(setupExpr, false);
        AssertIsNotSetUp();
        _context.SetDefault(setup);
    }

    internal void SetDefault<TValue>(
        Func<TValue, TValue> transform, string transformExpr)
    {
        SpecificationGenerator.AddGiven<TValue>(transformExpr, false);
        AssertIsNotSetUp();
        _context.SetDefault(transform);
    }

    internal void SetDefault<TValue>(TValue defaultValue, Scope scope, string defaultValuesExpr)
    {
        if (!string.IsNullOrEmpty(defaultValuesExpr))
            SpecificationGenerator.AddGiven(defaultValuesExpr, scope);
        AssertIsNotSetUp();
        _context.Use(defaultValue, scope);
    }

    internal void SetUnique<TValue>()
    {
        SpecificationGenerator.AddUnique<TValue>();
        AssertIsNotSetUp();
        _context.SetUnique<TValue>();
    }

    internal void PrependSetUp(Delegate setUp, string setUpExpr)
    {
        AssertIsNotSetUp();
        _fixture.After(new(setUp ?? throw new SetupFailed("SetUp cannot be null"), setUpExpr));
    }

    internal void SetTearDown(Delegate tearDown, string tearDownExpr)
    {
        AssertIsNotSetUp();
        _fixture.Before(new(tearDown ?? throw new SetupFailed("TearDown cannot be null"), tearDownExpr));
    }

    internal Lazy<TSUT> Arrange()
    {
        _arranger.Arrange();
        return new Lazy<TSUT>(Instantiate<TSUT>);
    }

    internal TClass Instantiate<TClass>() => _context.Instantiate<TClass>();

    internal Mock<TObject> GetMock<TObject>() where TObject : class
        => _context.GetMock<TObject>();

    internal void ArrangeFirst(Action arrangement)
    {
        AssertIsNotSetUp();
        _arranger.Push(arrangement);
    }

    internal void ArrangeLast(Action arrangement)
    {
        AssertIsNotSetUp();
        _arranger.Add(arrangement);
    }

    internal void SetupThrows<TService>(Func<Exception> expected)
    {
        AssertIsNotSetUp();
        _context.SetupThrows<TService>(expected);
    }

    internal void Register<TTarget, TSource>(Func<TSource, TTarget>? convert = null) 
        => _context.Register(convert);

    private void AssertIsNotSetUp()
    {
        if (_fixture.IsSetUp)
            throw new SetupFailed("Cannot provide setup after pipeline is set up");
    }
}