using HarmonyLib;
using InventorySystem.Searching;
using Qurre.API.Controllers.Items;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(ArmorSearchCompletor), nameof(ArmorSearchCompletor.Complete))]
    internal static class PickupArmor
    {
        private static bool Prefix(ArmorSearchCompletor __instance)
        {
            try
            {
                var ev = new PickupItemEvent(Player.Get(__instance.Hub), Pickup.Get(__instance.TargetPickup));
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
                Log.Error($"umm, error in patching Player [PickupArmor]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}