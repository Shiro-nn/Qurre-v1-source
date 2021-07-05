using HarmonyLib;
using UnityEngine;
namespace Qurre.Patches.etc
{

    [HarmonyPatch(typeof(PlayerMovementSync), "AnticheatIsIntersecting")]
    internal static class ScaleFix1
    {
        private static void Postfix(PlayerMovementSync __instance, ref bool __result)
        {
            if (__instance.transform.localScale.y != 1) __result = false;
        }
    }
    [HarmonyPatch(typeof(PlayerMovementSync), "AnticheatRaycast")]
    internal static class ScaleFix2
    {
        private static void Prefix(PlayerMovementSync __instance, ref Vector3 offset) => offset.y *= __instance.transform.localScale.y;
    }
}