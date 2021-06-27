using System;
using HarmonyLib;
using Qurre.API.Events;
using Searching;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickupItem
    {
        private static bool Prefix(ItemSearchCompletor __instance)
        {
            try
            {
                var item = __instance.ItemSearchCompletor_TargetPickup();
                if (item == null) return true;
                var ev = new PickupItemEvent(API.Player.Get(__instance.ItemSearchCompletor_Hub().gameObject), item);
                Qurre.Events.Invoke.Player.PickupItem(ev);
                if (!ev.Allowed) __instance.ItemSearchCompletor_TargetPickup().InUse = false;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [PickupItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}