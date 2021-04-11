#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.CallCmdDropItem))]
    internal static class DropItem
    {
        private static bool Prefix(Inventory __instance, int itemInventoryIndex)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true) || itemInventoryIndex < 0 ||
                    itemInventoryIndex >= __instance.items.Count) return false;
                Inventory.SyncItemInfo item = __instance.items[itemInventoryIndex];
                if (__instance.items[itemInventoryIndex].id != item.id) return false;
                var ev = new DroppingItemEvent(API.Player.Get(__instance.gameObject), item);
                Qurre.Events.Player.droppingItem(ev);
                item = ev.Item;
                if (!ev.Allowed) return false;
                Pickup pick = __instance.SetPickup(item.id, item.durability, __instance.transform.position, __instance.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
                __instance.items.RemoveAt(itemInventoryIndex);
                var ev1 = new DropItemEvent(API.Player.Get(__instance.gameObject), pick);
                Qurre.Events.Player.dropItem(ev1);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DropItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}