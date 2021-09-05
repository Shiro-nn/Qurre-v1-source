using HarmonyLib;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Controllers.Items;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.Complete))]
    internal static class PickupAmmo
    {
        private static bool Prefix(AmmoSearchCompletor __instance)
        {
            try
            {
                var pickup = Pickup.Get(__instance.TargetPickup);
                var ev = new PickupItemEvent(Player.Get(__instance.Hub), pickup);
                Qurre.Events.Invoke.Player.PickupItem(ev);
                if (!ev.Allowed)
                {
                    __instance.TargetPickup.Info.InUse = false;
                    __instance.TargetPickup.Info.Locked = false;
                    __instance.TargetPickup.NetworkInfo = __instance.TargetPickup.Info;
                }
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [PickupAmmo]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}