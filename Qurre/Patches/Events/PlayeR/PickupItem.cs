#pragma warning disable SA1313
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
                var ev = new PickupItemEvent(ReferenceHub.GetHub(__instance.ItemSearchCompletor_Hub().gameObject), __instance.ItemSearchCompletor_TargetPickup());
                Qurre.Events.Player.pickupItem(ev);
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.PickupItem:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}