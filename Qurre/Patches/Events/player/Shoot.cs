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
            try
            {
                if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub referenceHub)) return false;
                Player pl = Player.Get(referenceHub);
                if (msg.ShooterWeaponSerial != pl.CurrentItem.SerialNumber) return false;
                Firearm firearm = pl.CurInstance as Firearm;
                if (firearm == null) return false;
                if (!firearm.ActionModule.ServerAuthorizeShot()) return false;
                var ev = new ShootingEvent(pl, msg);
                Qurre.Events.Invoke.Player.Shooting(ev);
                try { if (ev.Allowed) firearm.HitregModule.ServerProcessShot(msg); } catch { }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Shoot]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}