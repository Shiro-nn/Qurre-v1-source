using HarmonyLib;
using Mirror;
using System;
using UnityEngine;
using static QurreModLoader.umm;

namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(ConsumableAndWearableItems), "LateUpdate")]
    internal static class MaxHpFix
    {
        private static bool Prefix(ConsumableAndWearableItems __instance)
        {
            try
            {
                if (NetworkServer.active && __instance.hpToHeal > 1f)
                {
                    if (__instance.CAWI_hub().playerStats.Health < __instance.CAWI_hub().characterClassManager.CurRole.maxHP ||
                        __instance.CAWI_hub().playerStats.Health < __instance.CAWI_hub().playerStats.maxHP)
                    {
                        __instance.CAWI_hub().playerStats.HealHPAmount(1f);
                    }
                    __instance.hpToHeal -= 1f;
                }
                if (__instance.isLocalPlayer)
                {
                    __instance.CAWI_cursor(Cursor.visible);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Fixes [MaxHp]:\n{e}\n{e.StackTrace}");
                return false;
            }
        }
    }
}