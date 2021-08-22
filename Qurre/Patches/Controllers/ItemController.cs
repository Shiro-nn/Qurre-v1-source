using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Controllers;
using System;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
    internal static class ItemController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
        internal static void AddItem(ItemBase __result, ushort itemSerial = 0, ItemPickupBase pickup = null)
        {
            try
            {
                if (itemSerial == 0 || pickup == null) new Item(__result);
                else
                {
                    var item = Item.AllItems[itemSerial];
                    item.ItemBase = __result;
                }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => AddItem]:\n{e}\n{e.StackTrace}");
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
        internal static void CreatePickup(ItemPickupBase __result, ItemBase item, bool spawn = true)
        {
            try
            {
                var _item = item.GetItem();
                if (_item == null) _item = new Item(__result);
                else _item.PickupBase = __result;
                if (!spawn) _item.PickupBase.transform.localScale = _item.Scale;
                else _item.Scale = _item.Scale;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => CreatePickup]:\n{e}\n{e.StackTrace}");
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerRemoveItem))]
        internal static void RemoveItem(ushort itemSerial, ItemPickupBase ipb)
        {
            try
            {
                if (ipb == null)
                {
                    var item = Item.AllItems[itemSerial];
                    item.Destroy();
                }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => RemoveItem]:\n{e}\n{e.StackTrace}");
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.CreatePickup))]
        internal static bool DistributorCreatePickup(ItemDistributor __instance, ItemType id, Transform t, string triggerDoor)
        {
            try
            {
                if (!InventoryItemLoader.AvailableItems.TryGetValue(id, out var itemBase)) return false;
                var itemPickupBase = UnityEngine.Object.Instantiate(itemBase.PickupDropModel, t.position, t.rotation);
                new Item(itemPickupBase);
                itemPickupBase.Info.ItemId = id;
                itemPickupBase.Info.Weight = itemBase.Weight;
                itemPickupBase.transform.SetParent(t);
                if (itemPickupBase is IPickupDistributorTrigger pickupDistributorTrigger) pickupDistributorTrigger.OnDistributed();
                if (string.IsNullOrEmpty(triggerDoor) || !Interactables.Interobjects.DoorUtils.DoorNametagExtension.NamedDoors.TryGetValue(triggerDoor, out var doorNametagExtension))
                {
                    ItemDistributor.SpawnPickup(itemPickupBase);
                    return false;
                }
                __instance.RegisterUnspawnedObject(doorNametagExtension.TargetDoor, itemPickupBase.gameObject);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => DistributorCreatePickup]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LockerChamber), nameof(LockerChamber.SpawnItem))]
        internal static bool ChamberSpawnItem(LockerChamber __instance, ItemType id, int amount)
        {
            try
            {
                if (id == ItemType.None || !InventoryItemLoader.AvailableItems.TryGetValue(id, out var itemBase)) return false;
                for (int i = 0; i < amount; i++)
                {
                    var itemPickupBase = UnityEngine.Object.Instantiate(itemBase.PickupDropModel, __instance._spawnpoint.position, __instance._spawnpoint.rotation);
                    new Item(itemPickupBase);
                    itemPickupBase.transform.SetParent(__instance._spawnpoint);
                    itemPickupBase.Info.ItemId = id;
                    itemPickupBase.Info.Weight = itemBase.Weight;
                    itemPickupBase.Info.Locked = true;
                    __instance._content.Add(itemPickupBase);
                    if (itemPickupBase is IPickupDistributorTrigger pickupDistributorTrigger) pickupDistributorTrigger.OnDistributed();
                    if (__instance._spawnOnFirstChamberOpening) __instance._toBeSpawned.Add(itemPickupBase);
                    else ItemDistributor.SpawnPickup(itemPickupBase);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => ChamberSpawnItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}