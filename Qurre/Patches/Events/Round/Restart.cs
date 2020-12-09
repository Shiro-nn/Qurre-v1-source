#pragma warning disable SA1313
using HarmonyLib;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Roundrestart))]
    internal static class Restart
    {
        private static void Prefix() => Qurre.Events.Round.restart();
    }
}