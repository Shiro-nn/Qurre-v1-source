using System;
using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerShotReceived))]
    internal static class Shoot
    {
        private static bool Prefix(NetworkConnection conn, ShotMessage msg)
        {
            try
            {
                Player pl = Player.Get(conn?.identity?.gameObject);
                if (pl is null) return true;
                if (msg.ShooterWeaponSerial != pl.Inventory.CurItem.SerialNumber) return false;
                if (pl.CurInstance is not Firearm firearm) return false;
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
                Log.Error($"umm, error in patching Player [Shoot]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}