using HarmonyLib;
using InventorySystem;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.ServerSelectItem))]
    internal static class ItemChange
    {
        internal static bool Prefix(Inventory __instance, ushort itemSerial)
        {
            try
            {
                if (itemSerial == __instance.CurItem.SerialNumber) return false;
                var ev = new ItemChangeEvent(Player.Get(__instance._hub), Item.Get(__instance.CurInstance), itemSerial == 0 ? null : Item.Get(itemSerial));
                Qurre.Events.Invoke.Player.ItemChange(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [ItemChange]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}