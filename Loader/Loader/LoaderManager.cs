using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
public static class LoaderManager
{
    public static string AppDataDir { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string QurreDir { get; private set; } = Path.Combine(AppDataDir, "Qurre");
    public static string PluginsDir { get; private set; } = Path.Combine(QurreDir, "Plugins");
    public static string DependDir { get; private set; } = Path.Combine(PluginsDir, "dependencies");
    public static byte[] ReadFile(string path) => File.ReadAllBytes(path);
    internal static bool Loaded(string a) => LocalLoaded.Any(x => x.Path == a);
    internal static readonly List<AssemblyDep> LocalLoaded = new();
}