using HarmonyLib;

namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdRegisterEscape")]
    internal static class EscapeFix
    {
        private static bool Prefix() => false;
    }
}