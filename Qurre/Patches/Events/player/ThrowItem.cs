using System;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(ThrowableNetworkHandler), nameof(ThrowableNetworkHandler.ServerProcessMessages))]
    internal static class ThrowItemPatch
    {
        private static bool Prefix(NetworkConnection conn, ref ThrowableNetworkHandler.ThrowableItemMessage msg)
        {
            try
            {
                ReferenceHub hub;
                if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out hub) || hub.inventory.CurItem.SerialNumber != msg.Serial || !(hub.inventory.CurInstance is ThrowableItem)) return false;
                var ev = new ThrowItemEvent(Player.Get(hub), msg.Serial.GetItem(), msg.Request);
                Qurre.Events.Invoke.Player.ThrowItem(ev);
                msg = new ThrowableNetworkHandler.ThrowableItemMessage(msg.Serial, ev.Request, msg.CameraRotation, msg.CameraPosition);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [ThrowItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}