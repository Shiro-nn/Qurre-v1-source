using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using Qurre.API;
using Qurre.API.Controllers;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal static class ItemController_Add
    {
        internal static void Postfix(ItemBase __result)
        {
            try
            {
                Item item = Item.Get(__result);
                if (item == null) return;
                Player pl = Player.Get(__result.Owner);
                pl?.ItemsValue.Add(item);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => AddItem]:\n{e}\n{e.StackTrace}");
            }
        }
    }
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropItem))]
    internal static class ItemController_Drop
    {
        internal static void Postfix(ushort itemSerial)
        {
            try
            {
                Item item = Item.Get(itemSerial);
                if (item == null) return;
                item.Owner?.ItemsValue.Remove(item);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => DropItem]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}