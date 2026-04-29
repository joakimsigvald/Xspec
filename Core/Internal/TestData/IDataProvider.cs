namespace Xspec.Internal.TestData;

internal interface IDataProvider
{
    Type[] UsedTypes { get; }
    bool TryGetValue(Type type, For scope, out object? val);
}