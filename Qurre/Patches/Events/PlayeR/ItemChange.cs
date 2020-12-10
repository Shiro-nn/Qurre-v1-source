#pragma warning disable SA1313
using HarmonyLib;
using Qurre.API.Events;
using Qurre.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.CallCmdSetUnic))]
    internal static class ItemChange
    {
        private static void Prefix(Inventory __instance, int i)
        {
            try
            {
                if (__instance.itemUniq == i)
                    return;
                int oI = __instance.GetItemIndex();
                if (oI == -1 && i == -1)
                    return;
                Inventory.SyncItemInfo old = oI == -1
                    ? new Inventory.SyncItemInfo() { id = ItemType.None }
                    : __instance.GetItemInHand();
                Inventory.SyncItemInfo newi = new Inventory.SyncItemInfo() { id = ItemType.None };
                foreach (Inventory.SyncItemInfo item in __instance.items)
                    if (item.uniq == i)
                        newi = item;
                var ev = new ItemChangeEvent(ReferenceHub.GetHub(__instance.gameObject), old, newi);
                Player.itemchange(ev);
                oI = __instance.GetItemIndex();
                if (oI != -1)
                    __instance.items[oI] = ev.OldItem;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.ItemChange:\n{e}\n{e.StackTrace}");
            }
        }
    }
}