#pragma warning disable SA1313
using HarmonyLib;
using ServerOutput;

namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
    internal static class Console
    {
        private static bool Prefix(string q)
        {
            if (q.ToString().Contains("[STDOUT]") && Log.AntiFlood) return false;
            else
            {
                Log.AllLogsTxt(q);
                return true;
            }
        }
    }
}