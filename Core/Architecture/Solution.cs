namespace Xspec.Architecture;

/// <summary>
/// The solution model that architecture rules are expressed over
/// </summary>
public sealed class Solution
{
    private readonly Dictionary<string, Project> _projectsByName;

    /// <summary>
    /// Create a solution model from the given projects, without touching disk
    /// </summary>
    /// <param name="projects">The projects of the solution</param>
    /// <param name="rootPath">The root directory of the solution, if any</param>
    /// <exception cref="SetupFailed">If two projects have the same name</exception>
    public Solution(IEnumerable<Project> projects, string? rootPath = null)
    {
        RootPath = rootPath;
        Projects = [.. projects];
        _projectsByName = new(StringComparer.Ordinal);
        foreach (var project in Projects)
            if (!_projectsByName.TryAdd(project.Name, project))
                throw new SetupFailed(
                    $"Solution contains two projects named '{project.Name}'"
                    + $" ({_projectsByName[project.Name].RelativePath}, {project.RelativePath})");
    }

    /// <summary>
    /// The root directory of the solution, or null for a fabricated solution
    /// </summary>
    public string? RootPath { get; }

    /// <summary>
    /// All projects of the solution
    /// </summary>
    public IReadOnlyList<Project> Projects { get; }

    /// <summary>
    /// Get a project by name
    /// </summary>
    /// <param name="name">The name of the project</param>
    /// <exception cref="SetupFailed">If the solution has no project with that name</exception>
    public Project this[string name]
        => _projectsByName.TryGetValue(name, out var project)
            ? project
            : throw new SetupFailed($"Solution contains no project named '{name}'");
}
