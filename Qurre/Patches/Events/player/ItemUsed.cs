using System;
using HarmonyLib;
using InventorySystem.Items.Usables;
namespace Qurre.Patches.Events.Player
{
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.ServerOnUsingCompleted))]
    internal static class ItemUsedPatch
    {
        private static void Prefix(Consumable __instance)
        {
            try { Qurre.Events.Invoke.Player.ItemUsed(new(API.Player.Get(__instance.Owner), __instance.OwnerInventory.CurItem)); }
            catch (Exception e) { Log.Error($"umm, error in patching Player [ItemUsed]:\n{e}\n{e.StackTrace}"); }
        }
    }
}