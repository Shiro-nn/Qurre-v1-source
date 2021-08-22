using HarmonyLib;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUsePanel))]
    internal static class NukeLock
    {
        private static bool Prefix() => !API.Controllers.Alpha.InsidePanel.Locked;
    }
}