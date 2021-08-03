using HarmonyLib;
using Qurre.API;
using System;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.GetShootPermission), new Type[] { typeof(CharacterClassManager), typeof(bool) })]
    internal static class FriendlyFire
    {
        private static bool Prefix(WeaponManager __instance, ref bool forceFriendlyFire, ref bool __result) =>
            !(__result = Player.Get(__instance.gameObject).FriendlyFire || forceFriendlyFire || Server.FriendlyFire);
    }
}