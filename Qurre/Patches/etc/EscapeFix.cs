using HarmonyLib;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallCmdRegisterEscape))]
    internal static class EscapeFix
    {
        private static bool Prefix() => false;
    }
}