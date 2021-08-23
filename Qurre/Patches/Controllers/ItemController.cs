using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Items;
using System;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal static class ItemController_Add
    {
        internal static void Postfix(ItemBase __result)
        {
            try
            {
                Player pl = Player.Get(__result.Owner);
                pl?.ItemsValue.Add(Item.Get(__result));
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
                if(item == null) return;
                item.Owner?.ItemsValue.Remove(item);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => DropItem]:\n{e}\n{e.StackTrace}");
            }
        }
    }
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    internal static class ItemController2
    {
        internal static void Postfix(ItemPickupBase __result, ItemBase item, bool spawn = true)
        {
            try
            {
                Pickup.Get(__result);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => CreatePickup]:\n{e}\n{e.StackTrace}");
            }
        }
    }/*
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerRemoveItem))]
    internal static class ItemController3
    {
        internal static void Postfix(ushort itemSerial, ItemPickupBase ipb)
        {
            try
            {

            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [ItemController => RemoveItem]:\n{e}\n{e.StackTrace}");
            }
        }
    }*/
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.CreatePickup))]
    internal static class ItemController4
    {
        internal static bool Prefix(ItemDistributor __instance, ItemType id, Transform t, string triggerDoor)
        {
            try
            {
                if (!InventoryItemLoader.AvailableItems.TryGetValue(id, out var itemBase)) return false;
                var itemPickupBase = UnityEngine.Object.Instantiate(itemBase.PickupDropModel, t.position, t.rotation);
                Pickup.Get(itemPickupBase);
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
    }
    [HarmonyPatch(typeof(LockerChamber), nameof(LockerChamber.SpawnItem))]
    internal static class ItemController5
    {
        internal static bool Prefix(LockerChamber __instance, ItemType id, int amount)
        {
            try
            {
                if (id == ItemType.None || !InventoryItemLoader.AvailableItems.TryGetValue(id, out var itemBase)) return false;
                for (int i = 0; i < amount; i++)
                {
                    var itemPickupBase = UnityEngine.Object.Instantiate(itemBase.PickupDropModel, __instance._spawnpoint.position, __instance._spawnpoint.rotation);
                    Pickup.Get(itemPickupBase);
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