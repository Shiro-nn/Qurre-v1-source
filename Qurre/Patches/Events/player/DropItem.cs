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
                var ev = new DroppingItemEvent(pl, Item.Get(itemSerial));
                Qurre.Events.Invoke.Player.DroppingItem(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DropItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        private static void Postfix(Inventory __instance, ushort itemSerial)
        {
            try
            {
                var _ev = new DropItemEvent(Player.Get(__instance._hub), Item.Get(itemSerial));
                Qurre.Events.Invoke.Player.DropItem(_ev);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DropItem]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}