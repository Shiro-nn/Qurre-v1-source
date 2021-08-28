using System;
using System.Collections.Generic;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem))]
    internal static class DropItem
    {
        internal static Dictionary<ushort, byte> Data = new Dictionary<ushort, byte>();
        private static bool Prefix(Inventory __instance, ushort itemSerial)
        {
            try
            {
                Player pl = Player.Get(__instance._hub);
                Item item = Item.Get(itemSerial);
                if (item == null) return false;
                if (pl == null) return false;
                var ev = new DroppingItemEvent(pl, item);
                Qurre.Events.Invoke.Player.DroppingItem(ev);
                try
                {
                    if (ev.Allowed && item.Base.Category == ItemCategory.Firearm)
                    {
                        if (Data.ContainsKey(itemSerial)) Data.Remove(itemSerial);
                        byte Durability = (item.Base as Firearm).Status.Ammo;
                        Data.Add(itemSerial, Durability);
                    }
                }
                catch { }
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
                Item item = Item.Get(itemSerial);
                var _ev = new DropItemEvent(Player.Get(__instance._hub), item);
                Qurre.Events.Invoke.Player.DropItem(_ev);
                try
                {
                    if (!Data.ContainsKey(itemSerial)) return;
                    var armpickup = API.Map.Pickups.Find(x => x.Serial == item.Serial).Base as FirearmPickup;
                    armpickup.Status = new FirearmStatus(Data[itemSerial], armpickup.Status.Flags, armpickup.Status.Attachments);
                }
                catch { }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DropItem]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}