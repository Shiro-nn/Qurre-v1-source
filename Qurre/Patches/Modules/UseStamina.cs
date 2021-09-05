using HarmonyLib;
using Qurre.API;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(Stamina), nameof(Stamina.ProcessStamina))]
    internal static class UseStamina
    {
        private static bool Prefix(Stamina __instance) => Player.Get(__instance._hub)?.UseStamina ?? true;
    }
}