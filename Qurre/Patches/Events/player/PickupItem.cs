using System;
using HarmonyLib;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Controllers.Items;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickupItem
    {
        private static bool Prefix(ItemSearchCompletor __instance)
        {
            try
            {
                var pickup = Pickup.Get(__instance.TargetPickup);
                var ev = new PickupItemEvent(Player.Get(__instance.Hub), pickup);
                Qurre.Events.Invoke.Player.PickupItem(ev);
                if (!ev.Allowed)
                {
                    var info = __instance.TargetPickup.Info;
                    info.InUse = false;
                    info.Locked = false;
                    __instance.TargetPickup.NetworkInfo = info;
                }
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