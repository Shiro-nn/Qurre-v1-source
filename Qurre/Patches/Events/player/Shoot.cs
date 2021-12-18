using System;
using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerShotReceived))]
    internal static class Shoot
    {
        private static bool Prefix(NetworkConnection conn, ShotMessage msg)
        {
            bool logging_error = true;
            try
            {
                try { _ = conn.identity.gameObject; } catch { logging_error = false; }
                if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub hub)) return false;
                Player pl = Player.Get(hub);
                if (msg.ShooterWeaponSerial != hub.inventory.CurItem.SerialNumber) return false;
                Firearm firearm = pl.CurInstance as Firearm;
                if (firearm == null) return false;
                var ev = new ShootingEvent(pl, msg);
                Qurre.Events.Invoke.Player.Shooting(ev);
                if (!ev.Allowed) return false;
                if (!firearm.ActionModule.ServerAuthorizeShot()) return false;
                firearm.HitregModule.ServerProcessShot(msg);
                firearm.OnWeaponShot();
                return false;
            }
            catch (Exception e)
            {
                if (logging_error) Log.Error($"umm, error in patching Player [Shoot]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}