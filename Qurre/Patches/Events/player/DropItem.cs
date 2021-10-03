using System;
using HarmonyLib;
using InventorySystem;
using Qurre.API;
using Qurre.API.Controllers;
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
                Player pl = Player.Get(__instance._hub);
                Item item = Item.Get(itemSerial);
                if (item == null || pl == null) return false;
                var ev = new DroppingItemEvent(pl, item);
                Qurre.Events.Invoke.Player.DroppingItem(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DropItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}