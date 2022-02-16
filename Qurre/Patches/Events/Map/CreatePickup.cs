using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    internal static class CreatePickup
    {
        internal static void Prefix(Inventory inv, ItemBase item, PickupSyncInfo psi, ref bool spawn)
        {
            try
            {
                var ev = new CreatePickupEvent(psi, inv, spawn);
                Qurre.Events.Invoke.Map.CreatePickup(ev);
                spawn = ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [CreatePickup]:\n{e}\n{e.StackTrace}");
            }
        }
    }
} 
