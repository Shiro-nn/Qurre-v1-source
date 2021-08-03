using HarmonyLib;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallRpcRoundStarted))]
    internal static class Start
    {
        private static void Postfix() => Qurre.Events.Invoke.Round.Start();
    }
}