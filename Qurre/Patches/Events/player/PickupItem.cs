using System;
using System.Collections.Generic;
using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Items;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickupItem
    {
        private static Dictionary<ushort, byte> Data => DropItem.Data;
        private static bool Prefix(ItemSearchCompletor __instance)
        {
            try
            {
                var pickup = Pickup.Get(__instance.TargetPickup);
                var ev = new PickupItemEvent(Player.Get(__instance.Hub), pickup);
                Qurre.Events.Invoke.Player.PickupItem(ev);
                if (!ev.Allowed)
                {
                    __instance.TargetPickup.Info.InUse = false;
                    __instance.TargetPickup.NetworkInfo = __instance.TargetPickup.Info;
                }
                try
                {
                    var item = Item.Get(__instance.TargetPickup.Info.Serial);
                    if (ev.Allowed && item.Base.Category == ItemCategory.Firearm)
                    {
                        byte Durability = (pickup.Base as FirearmPickup).Status.Ammo;
                        if (Data.ContainsKey(item.Serial)) Data.Remove(pickup.Serial);
                        Data.Add(pickup.Serial, Durability);
                    }
                }
                catch { }
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [PickupItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        private static void Postfix(ItemSearchCompletor __instance)
        {
            try
            {
                var item = Item.Get(__instance.TargetPickup.Info.Serial);
                if (!Data.ContainsKey(item.Serial)) return;
                var arm = item.Base as InventorySystem.Items.Firearms.Firearm;
                arm.Status = new FirearmStatus(Data[item.Serial], arm.Status.Flags, arm.Status.Attachments);
                Data.Remove(item.Serial);
            }
            catch { }
        }
    }
}