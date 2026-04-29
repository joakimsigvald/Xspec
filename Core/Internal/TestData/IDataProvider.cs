namespace Xspec.Internal.TestData;

internal interface IDataProvider
{
    Type[] UsedTypes { get; }
    (object? val, bool found) Use(Type type, For scope);
    object Create(Type type, For scope);
    bool TryGetValue(Type type, For scope, out object? val);
}