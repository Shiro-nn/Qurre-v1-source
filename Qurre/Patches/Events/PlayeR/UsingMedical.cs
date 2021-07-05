using System;
using HarmonyLib;
using MEC;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdUseMedicalItem))]
    internal static class UsingMedical
    {
        private static bool Prefix(ConsumableAndWearableItems __instance)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true)) return false;
                __instance.CAWI_cancel(false);
                if (__instance.cooldown > 0.0) return false;
                for (int i = 0; i < __instance.usableItems.Length; ++i)
                {
                    if (__instance.usableItems[i].inventoryID == __instance.CAWI_hub().inventory.curItem &&
                        __instance.usableCooldowns[i] <= 0.0)
                    {
                        var ev = new MedicalUsingEvent(API.Player.Get(__instance.gameObject), __instance.CAWI_hub().inventory.curItem, __instance.usableItems[i].animationDuration);
                        Qurre.Events.Invoke.Player.MedicalUsing(ev);
                        __instance.cooldown = ev.Cooldown;
                        if (ev.Allowed) Timing.RunCoroutine(__instance.UseMedicalItem(i), Segment.FixedUpdate);
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [UsingMedical]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}