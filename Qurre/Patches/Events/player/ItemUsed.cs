using System;
using HarmonyLib;
using InventorySystem.Items.Usables;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.ServerOnUsingCompleted))]
    internal static class ItemUsedPatch
    {
        private static void Prefix(Consumable __instance)
        {
            try
            {
                var ev = new ItemUsedEvent(API.Player.Get(__instance.Owner), __instance.OwnerInventory.CurItem);
                Qurre.Events.Invoke.Player.ItemUsed(ev);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [ItemUsed]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}