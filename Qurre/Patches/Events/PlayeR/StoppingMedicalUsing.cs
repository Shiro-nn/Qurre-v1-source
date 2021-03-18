#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdCancelMedicalItem))]
    internal static class StoppingMedicalUsing
    {
        private static bool Prefix(ConsumableAndWearableItems __instance)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true)) return false;
                for (int i = 0; i < __instance.usableItems.Length; ++i)
                {
                    if (__instance.usableItems[i].inventoryID == __instance.CAWI_hub().inventory.curItem && __instance.usableItems[i].cancelableTime > 0f)
                    {
                        var ev = new MedicalStoppingEvent(API.Player.Get(__instance.gameObject), __instance.CAWI_hub().inventory.curItem, __instance.usableItems[i].animationDuration);
                        Qurre.Events.Player.medicalStopping(ev);
                        __instance.CAWI_cancel(ev.Allowed);
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.StoppingMedicalUsing:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}