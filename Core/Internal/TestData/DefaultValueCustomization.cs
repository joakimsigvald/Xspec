namespace Xspec.Internal.TestData;

internal class DefaultValueCustomization(DataProvider _context) : IDefaultProvider
{
    public bool TryGetDefault(Type type, out object? val)
    {
        if (_context.TryGetDefault(type, out val))
            return true;

        if (type.IsInterface)
        {
            var (instance, found) = _context.Use(type);
            try
            {
                val = found ? instance! : _context.GetMock(type).Object;
                return true;
            }
            catch { }
        }
        val = null;
        return false;
    }
}