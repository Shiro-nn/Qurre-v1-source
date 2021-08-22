using HarmonyLib;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_CmdRegisterEscape))]
    internal static class EscapeFix
    {
        private static bool Prefix() => false;
    }
}