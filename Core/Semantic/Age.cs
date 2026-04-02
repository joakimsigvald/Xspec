namespace Xspec.Semantic;

/// <summary>
/// Represents an age below 120
/// </summary>
/// <param name="Value"></param>
public record Age(int Value) : Semantic<int>(Value), ISemantic<int>
{
    /// <summary>
    /// Generates an age below 120
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static int GenerateValue(int seed) => seed % 120;
}