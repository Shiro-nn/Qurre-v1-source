using System;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem))]
    internal static class DropItem
    {
        private static bool Prefix(Inventory __instance, ushort itemSerial, bool tryThrow)
        {
            try
            {
                if (!__instance.UserInventory.Items.TryGetValue(itemSerial, out ItemBase itemBase) || !itemBase.CanHolster()) return false;
                Player pl = Player.Get(__instance._hub);
                var ev = new DroppingItemEvent(pl, itemBase);
                Qurre.Events.Invoke.Player.DroppingItem(ev);
                if (!ev.Allowed) return false;
                itemSerial = ev.Item.ItemSerial;
                InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = __instance.ServerDropItem(itemSerial);
                __instance.SendItemsNextFrame = true;
                if (tryThrow && itemPickupBase != null && itemPickupBase.TryGetComponent(out Rigidbody rigidbody))
                {
                    Vector3 vector = __instance._hub.playerMovementSync.PlayerVelocity / 3f + pl.Rotation * 6f * (Mathf.Clamp01(Mathf.InverseLerp(7f, 0.1f, rigidbody.mass)) + 0.3f);
                    vector.x = Mathf.Max(Mathf.Abs(__instance._hub.playerMovementSync.PlayerVelocity.x), Mathf.Abs(vector.x)) * ((vector.x < 0f) ? -1 : 1);
                    vector.y = Mathf.Max(Mathf.Abs(__instance._hub.playerMovementSync.PlayerVelocity.y), Mathf.Abs(vector.y)) * ((vector.y < 0f) ? -1 : 1);
                    vector.z = Mathf.Max(Mathf.Abs(__instance._hub.playerMovementSync.PlayerVelocity.z), Mathf.Abs(vector.z)) * ((vector.z < 0f) ? -1 : 1);
                    rigidbody.position = __instance._hub.PlayerCameraReference.position;
                    rigidbody.velocity = vector;
                    rigidbody.angularVelocity = Vector3.Lerp(itemBase.ThrowSettings.RandomTorqueA, itemBase.ThrowSettings.RandomTorqueB, UnityEngine.Random.value);
                    float magnitude = rigidbody.angularVelocity.magnitude;
                    if (magnitude > rigidbody.maxAngularVelocity)
                    {
                        rigidbody.maxAngularVelocity = magnitude;
                    }
                    var _ev = new DropItemEvent(pl, itemPickupBase.GetItem());
                    Qurre.Events.Invoke.Player.DropItem(_ev);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DropItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}