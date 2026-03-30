namespace Xspec.Internal.TestData;

internal interface IDefaultProvider
{
    bool TryGetDefault(Type type, out object? val);
}