using HarmonyLib;
using InventorySystem.Items.Pickups;
using Qurre.API.Controllers.Items;
using Qurre.API.Events;
using Scp914;
using Scp914.Processors;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp914
{
    [HarmonyPatch(typeof(FirearmItemProcessor), nameof(FirearmItemProcessor.OnPickupUpgraded))]
    internal static class UpgradedItemPickup1
    {
        private static bool Prefix(ref Scp914KnobSetting setting, ItemPickupBase ipb, ref Vector3 newPos)
        {
            try
            {
                var item = Pickup.Get(ipb);
                var ev = new UpgradedItemPickupEvent(item, newPos, setting);
                Qurre.Events.Invoke.Scp914.UpgradedItemPickup(ev);
                newPos = ev.Position;
                setting = ev.Setting;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [UpgradedItemPickup #1]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(StandardItemProcessor), nameof(StandardItemProcessor.OnPickupUpgraded))]
    internal static class UpgradedItemPickup2
    {
        private static bool Prefix(ref Scp914KnobSetting setting, ItemPickupBase ipb, ref Vector3 newPosition)
        {
            try
            {
                var item = Pickup.Get(ipb);
                var ev = new UpgradedItemPickupEvent(item, newPosition, setting);
                Qurre.Events.Invoke.Scp914.UpgradedItemPickup(ev);
                newPosition = ev.Position;
                setting = ev.Setting;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [UpgradedItemPickup #2]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(AmmoItemProcessor), nameof(AmmoItemProcessor.OnPickupUpgraded))]
    internal static class UpgradedItemPickup3
    {
        private static bool Prefix(ref Scp914KnobSetting setting, ItemPickupBase ipb, ref Vector3 newPos)
        {
            try
            {
                var item = Pickup.Get(ipb);
                var ev = new UpgradedItemPickupEvent(item, newPos, setting);
                Qurre.Events.Invoke.Scp914.UpgradedItemPickup(ev);
                newPos = ev.Position;
                setting = ev.Setting;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [UpgradedItemPickup #3]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}