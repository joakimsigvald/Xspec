namespace Xspec.Architecture;

/// <summary>
/// One project of the solution, as the architecture rules see it
/// </summary>
public sealed record Project
{
    /// <summary>
    /// The project name (the csproj file name without extension), unique within the solution
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The path of the csproj file relative to the solution root, with forward slashes
    /// </summary>
    public string RelativePath { get; init; } = string.Empty;

    /// <summary>
    /// The names of the projects that this project references directly
    /// </summary>
    public IReadOnlyList<string> ProjectReferences { get; init; } = [];

    /// <summary>
    /// The ids of the nuget packages that this project references directly
    /// </summary>
    public IReadOnlyList<string> PackageReferences { get; init; } = [];

    private readonly string? _rootNamespace;

    /// <summary>
    /// The root namespace of the project, defaulting to the project name
    /// </summary>
    public string RootNamespace
    {
        get => _rootNamespace ?? Name;
        init => _rootNamespace = value;
    }

    /// <summary>
    /// The directory of the csproj file relative to the solution root, with forward slashes
    /// </summary>
    public string Directory
        => RelativePath.LastIndexOf('/') is var separatorIndex and >= 0
            ? RelativePath[..separatorIndex]
            : string.Empty;

    /// <summary>
    /// All direct references of the project (projects and packages)
    /// </summary>
    public IEnumerable<string> References => ProjectReferences.Concat(PackageReferences);

    /// <returns>The project name</returns>
    public override string ToString() => Name;
}
