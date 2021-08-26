using Scp914;
using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API;
using Qurre.API.Events;
using System.Collections.Generic;
using NorthwoodLib.Pools;
using InventorySystem.Items.Pickups;
namespace Qurre.Patches.Events.SCPs.Scp914
{
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.Upgrade))]
    internal static class Upgrade
    {
        private static bool Prefix(Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
        {
            try
            {
                if (!NetworkServer.active) return true;
                HashSet<GameObject> hashSet = HashSetPool<GameObject>.Shared.Rent();
                bool upgradeDropped = (mode & Scp914Mode.Dropped) == Scp914Mode.Dropped;
                bool upgradeInventory = (mode & Scp914Mode.Inventory) == Scp914Mode.Inventory;
                bool heldOnly = upgradeInventory && (mode & Scp914Mode.Held) == Scp914Mode.Held;
                var players = new List<Player>();
                var items = new List<ItemPickupBase>();
                foreach (var collider in intake)
                {
                    var gameObject = collider.transform.root.gameObject;
                    if (hashSet.Add(gameObject))
                    {
                        if (ReferenceHub.TryGetHub(gameObject, out var ply)) players.Add(Player.Get(ply));
                        else if (gameObject.TryGetComponent<ItemPickupBase>(out var pickup)) items.Add(pickup);
                    }
                }
                var ev = new UpgradeEvent(players, items, moveVector, mode, setting);
                Qurre.Events.Invoke.Scp914.Upgrade(ev);
                if (!ev.Allowed) return false;
                players = ev.Players;
                items = ev.Items;
                moveVector = ev.Move;
                mode = ev.Mode;
                setting = ev.Setting;
                foreach (var ply in players) Scp914Upgrader.ProcessPlayer(ply.ReferenceHub, upgradeInventory, heldOnly, moveVector, setting);
                foreach (var item in items) Scp914Upgrader.ProcessPickup(item, upgradeDropped, moveVector, setting);
                HashSetPool<GameObject>.Shared.Return(hashSet);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [Upgrade]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}