#pragma warning disable SA1313
using HarmonyLib;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CmdStartRound))]
    internal static class Start
    {
        private static void Postfix() => Qurre.Events.Round.start();
    }
}