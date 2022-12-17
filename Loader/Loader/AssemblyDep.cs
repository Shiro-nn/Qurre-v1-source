using System.Reflection;
internal struct AssemblyDep
{
    internal readonly Assembly Assembly { get; }
    internal readonly string Path { get; }
    internal AssemblyDep(Assembly assembly, string path)
    {
        Assembly = assembly;
        Path = path;
    }
}