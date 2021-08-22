using Scp914;
using HarmonyLib;
using UnityEngine;
using Qurre.API;
using Qurre.API.Events;
using InventorySystem.Items.Pickups;

namespace Qurre.Patches.Events.SCPs.SCP914
{
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPlayer))]
    internal static class UpgradePlayer
    {
        private static bool Prefix(ReferenceHub ply, ref bool upgradeInventory, ref bool heldOnly, ref Vector3 moveVector, ref Scp914KnobSetting setting)
        {
            try
            {
                var ev = new UpgradePlayerEvent(Player.Get(ply), upgradeInventory, heldOnly, moveVector, setting);
                Qurre.Events.Invoke.Scp914.UpgradePlayer(ev);
                upgradeInventory = ev.UpgradeInventory;
                heldOnly = ev.HeldOnly;
                moveVector = ev.Move;
                setting = ev.Setting;
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [UpgradePlayer]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPickup))]
    internal static class UpgradePickup
    {
        private static bool Prefix(ItemPickupBase pickup, ref bool upgradeDropped, ref Vector3 moveVector, ref Scp914KnobSetting setting)
        {
            try
            {
                var ev = new UpgradePickupEvent(pickup, upgradeDropped, moveVector, setting);
                Qurre.Events.Invoke.Scp914.UpgradePickup(ev);
                upgradeDropped = ev.UpgradeDropped;
                moveVector = ev.Move;
                setting = ev.Setting;
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [UpgradePlayer]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}