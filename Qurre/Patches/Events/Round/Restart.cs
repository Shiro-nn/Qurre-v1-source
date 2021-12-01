using HarmonyLib;
using RoundRestarting;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))]
    internal static class Restart
    {
        private static void Prefix() => Qurre.Events.Invoke.Round.Restart();
    }
}