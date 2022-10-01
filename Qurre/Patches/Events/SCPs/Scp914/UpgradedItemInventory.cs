using HarmonyLib;
using Qurre.API.Controllers;
using Qurre.API.Events;
using Scp914;
using Scp914.Processors;
using System;
namespace Qurre.Patches.Events.SCPs.Scp914
{
    using Qurre.API;
    [HarmonyPatch(typeof(FirearmItemProcessor), nameof(FirearmItemProcessor.OnInventoryItemUpgraded))]
    internal static class UpgradedItemInventory1
    {
        private static bool Prefix(ref Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            try
            {
                var item = Item.Get(serial);
                var ev = new UpgradedItemInventoryEvent(item, Player.Get(hub), setting);
                Qurre.Events.Invoke.Scp914.UpgradedItemInventory(ev);
                setting = ev.Setting;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [UpgradedItemInventory #1]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(StandardItemProcessor), nameof(StandardItemProcessor.OnInventoryItemUpgraded))]
    internal static class UpgradedItemInventory2
    {
        private static bool Prefix(ref Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            try
            {
                var item = Item.Get(serial);
                var ev = new UpgradedItemInventoryEvent(item, Player.Get(hub), setting);
                Qurre.Events.Invoke.Scp914.UpgradedItemInventory(ev);
                setting = ev.Setting;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [UpgradedItemInventory #2]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}