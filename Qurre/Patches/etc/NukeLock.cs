using HarmonyLib;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUsePanel))]
    internal static class NukeLock
    {
        private static bool Prefix() => !API.Controllers.Alpha.InsidePanel.Locked;
    }
}