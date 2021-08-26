using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Pickups;
using System;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.DestroyItemInstance))]
    internal static class FixItemDestroy
    {
        internal static bool Prefix(Inventory __instance, ref bool __result, ushort targetInstance, ItemPickupBase pickup)
        {
            try
            {
                if (!__instance.UserInventory.Items.TryGetValue(targetInstance, out InventorySystem.Items.ItemBase itemBase))
                {
                    __result = false;
                    return false;
                }
                if (itemBase == null || itemBase.gameObject == null)
                {
                    __result = false;
                    return false;
                }
                itemBase.OnRemoved(pickup);
                if (__instance.CurInstance == itemBase) __instance.CurInstance = null;
                UnityEngine.Object.Destroy(itemBase.gameObject);
                __result = true;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Modules [FixItemDestroy]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}