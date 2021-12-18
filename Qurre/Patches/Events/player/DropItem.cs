using System;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Items;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem))]
    internal static class DropItem
    {
        private static bool Prefix(Inventory __instance, ushort itemSerial)
        {
            try
            {
                if (!__instance.UserInventory.Items.TryGetValue(itemSerial, out ItemBase value) || !value.CanHolster()) return false;
                Player pl = Player.Get(__instance._hub);
                Item item = Item.Get(value);
                if (item == null || pl == null) return false;
                var ev = new DroppingItemEvent(pl, item);
                Qurre.Events.Invoke.Player.DroppingItem(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DroppingItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        private static void Postfix(Inventory __instance, ushort itemSerial)
        {
            try
            {
                if (!__instance.UserInventory.Items.TryGetValue(itemSerial, out ItemBase value) || !value.CanHolster()) return;
                Player pl = Player.Get(__instance._hub);
                Pickup pick = API.Map.Pickups.Find(x => x.Serial == itemSerial);
                if (pl == null || pick == null) return;
                Qurre.Events.Invoke.Player.DropItem(new DropItemEvent(pl, pick));
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DroppingItem]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}