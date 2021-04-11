using HarmonyLib;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
    internal static class Console
    {
        private static void Postfix(string q) => Log.AllLogsTxt(q);
    }
}