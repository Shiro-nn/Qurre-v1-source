using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    internal static class CreatePickup
    {
        internal static bool Prefix(ref ItemPickupBase __result, Inventory inv, ItemBase item, PickupSyncInfo psi, bool spawn = true)
        {
            try
            {
                __result = null;
                var ev = new CreatePickupEvent(psi, inv, spawn);
                Qurre.Events.Invoke.Map.CreatePickup(ev);
                if (!ev.Allowed) return false;
                ItemPickupBase itemPickupBase = UnityEngine.Object.Instantiate(item.PickupDropModel,
                    inv.transform.position, ReferenceHub.GetHub(inv.gameObject).PlayerCameraReference.rotation * item.PickupDropModel.transform.rotation);
                itemPickupBase.NetworkInfo = psi;
                itemPickupBase.Info = psi;
                NetworkServer.Spawn(itemPickupBase.gameObject);
                itemPickupBase.InfoReceived(default, psi);
                __result = itemPickupBase;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [CreatePickup]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}